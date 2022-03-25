using InvoiceAssessmentApi.Models;
using InvoiceAssessmentApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace InvoiceAssessmentApi.Functions
{
    public class UpdateInvoice
    {
        #region Members

        private readonly ILogger<UpdateInvoice> logger;
        private readonly IInvoiceService invoiceService;

        #endregion

        #region Public

        /// <summary>
        /// UpdateInvoice Constructor.
        /// </summary>
        public UpdateInvoice(
            ILogger<UpdateInvoice> log,
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
        /// PUT api/Invoice/{invoiceId}
        /// ]]>.
        /// </example>
        [FunctionName(nameof(UpdateInvoice))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Invoice/{invoiceId}")] HttpRequest req, int invoiceId)

        {
            IActionResult result;

            try
            {
                Invoice invoiceToUpdate = await invoiceService.GetInvoice(invoiceId, null);

                if (invoiceToUpdate == null)
                {
                   logger.LogWarning($"Invoice with invoiceId: {invoiceId} was not found.");
                    result = new StatusCodeResult(StatusCodes.Status404NotFound);
                }

                var input = await new StreamReader(req.Body).ReadToEndAsync();

                Invoice updateInvoiceRequest = JsonConvert.DeserializeObject<Invoice>(input);

                this.GenerateInvoice(invoiceToUpdate, updateInvoiceRequest);
                
                await invoiceService.UpdateInvoice(invoiceToUpdate.InvoiceId, updateInvoiceRequest);

                result = new StatusCodeResult(StatusCodes.Status202Accepted);
            }
            catch (Exception ex)
            {
                logger.LogError($"Internal Server Error: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }

        #endregion

        #region Private

        /// <summary>
        /// Manages the calculated Invoice properties. 
        /// </summary>
        /// <param name="updateInvoiceRequest">The invoice request.</param>
        /// <param name="invoiceToUpdate">The invoice to update.</param>
        /// <returns>The awaited Task.</returns>
        private void GenerateInvoice(Invoice invoiceToUpdate, Invoice updateInvoiceRequest)
        {
            updateInvoiceRequest.Id = invoiceToUpdate.Id;
            updateInvoiceRequest.InvoiceId = invoiceToUpdate.InvoiceId;
        }

        #endregion
    }
}
