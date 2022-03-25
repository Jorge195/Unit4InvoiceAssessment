using InvoiceAssessmentApi.Models;
using InvoiceAssessmentApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceAssessmentApi.Functions
{
    /// <summary>
    /// Endpoint for creating invoices. 
    /// </summary>
    public class CreateInvoice
    {
        #region Members

        private readonly ILogger<CreateInvoice> logger;
        private readonly IInvoiceService invoiceService;

        #endregion

        #region Public

        /// <summary>
        /// CreateInvoice Constructor.
        /// </summary>
        public CreateInvoice(
          ILogger<CreateInvoice> log,
          IInvoiceService invService)
        {
            logger = log;
            invoiceService = invService;
        }

        /// <summary>
        /// Endpoint for creating an Invoice.
        /// </summary>
        /// <returns>
        /// The entity resource with all the default values set as needed.
        /// </returns>
        /// <example>
        /// <![CDATA[
        /// POST api/Invoice
        /// ]]>.
        /// </example>
        [FunctionName(nameof(CreateInvoice))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Invoice")] HttpRequest req)
        {
            IActionResult result;

            try
            {
                var incomingRequest = await new StreamReader(req.Body).ReadToEndAsync();

                Invoice invoiceRequest = JsonConvert.DeserializeObject<Invoice>(incomingRequest);

                await this.GenerateInvoice(invoiceRequest);

                await invoiceService.CreateInvoice(invoiceRequest);

                result = new StatusCodeResult(StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                logger.LogError($"Internal Server Error. Exception: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }

        #endregion

        #region Private

        /// <summary>
        /// Manages the calculated Invoice properties. 
        /// </summary>
        /// <param name="invoiceRequest">The invoice request.</param>
        /// <returns>The awaited Task.</returns>
        private async Task GenerateInvoice(Invoice invoiceRequest)
        {
            invoiceRequest.Id = ObjectId.GenerateNewId().ToString();
            invoiceRequest.InvoiceId = await ManageInvoiceInvoiceId();
        }

        /// <summary>
        /// Responsible for obtaining a sequencial and unique series number. 
        /// </summary>
        /// <returns>The next number in the invoice sequence.</returns>
        private async Task<int> ManageInvoiceInvoiceId()
        {
            int currentInvoiceId = 1;

            List<Invoice> invoices = await invoiceService.GetInvoices();

            if (invoices.Any())
            {
                currentInvoiceId = invoices.Max(t => t.InvoiceId);
                currentInvoiceId++;
            }

            return currentInvoiceId;
        }

        #endregion
    }
}
