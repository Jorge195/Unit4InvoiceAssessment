using InvoiceAssessmentApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace InvoiceAssessmentApi.Functions
{
    /// <summary>
    /// Endpoint for getting a list of invoices. 
    /// </summary>
    public class GetInvoices
    {
        #region Members

        private readonly ILogger<GetInvoices>logger;
        private readonly IInvoiceService invoiceService;

        #endregion

        #region Public

        /// <summary>
        /// GetInvoices Constructor.
        /// </summary>
        public GetInvoices
            (ILogger<GetInvoices> log,
            IInvoiceService invService)
        {
            logger = log;
            invoiceService = invService;
        }
        /// <summary>
        /// Endpoint for getting a collection of invoices.
        /// </summary>
        /// <returns>
        /// The list of all invoices.
        /// </returns>
        /// <example>
        /// <![CDATA[
        /// GET api/Invoices
        /// ]]>.
        /// </example>
        [FunctionName(nameof(GetInvoices))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Invoices")] HttpRequest req)
        {
            IActionResult result;

            try
            {
                var invoices = await invoiceService.GetInvoices();

                if (invoices == null)
                {
                   logger.LogWarning("No invoices found!");
                    result = new StatusCodeResult(StatusCodes.Status404NotFound);
                }

                result = new OkObjectResult(invoices);
            }
            catch (Exception ex)
            {
                logger.LogError($"Internal Server Error. Exception thrown: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }

        #endregion
    }
}
