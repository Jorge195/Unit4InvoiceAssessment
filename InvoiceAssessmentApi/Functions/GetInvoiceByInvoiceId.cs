using InvoiceAssessmentApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace InvoiceAssessmentApi.Functions
{
    public class GetInvoiceByInvoiceId
    {
        #region Members

        private readonly ILogger<GetInvoiceByInvoiceId> logger;
        private readonly IInvoiceService invoiceService;

        #endregion

        #region Public

        /// <summary>
        /// GetInvoiceByInvoiceId Constructor.
        /// </summary>
        public GetInvoiceByInvoiceId(
            ILogger<GetInvoiceByInvoiceId> log,
            IInvoiceService invService)
        {
            logger = log;
            invoiceService = invService;
        }

        /// <summary>
        /// Endpoint for getting an Invoice by invoiceId.
        /// </summary>
        /// <returns>
        /// The invoice found, and its respective Amount based on the currency.
        /// </returns>
        /// <example>
        /// <![CDATA[
        /// GET api/Invoice/{invoiceId}/{currency?}
        /// ]]>.
        /// </example>
        [FunctionName(nameof(GetInvoiceByInvoiceId))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Invoice/{invoiceId}/{currency?}")] HttpRequest req, int invoiceId, string currency)
        {
            IActionResult result;

            try
            {
                var invoice = await invoiceService.GetInvoice(invoiceId, currency);

                if (invoice == null)
                {
                    logger.LogWarning($"Invoice with invoiceId: {invoiceId} was not found.");
                    result = new StatusCodeResult(StatusCodes.Status404NotFound);
                }

                // If the request currency is provided, we want the response in the specified curreny and converted amount.
                if (currency != null)
                {
                    invoice.Amount = await CalculateExchangeRateAsync(invoice.Amount, invoice.Currency, currency);
                    invoice.Currency = currency;
                }

                result = new OkObjectResult(invoice);
            }
            catch (Exception ex)
            {
                logger.LogError($"Internal Server Error. Exception thrown: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }

        #endregion

        #region Private

        /// <summary>
        /// Calculates the Amount based on the exchange rate between the target and based currency. 
        /// </summary>
        /// <param name="amount">The invoice amount in the base currency./></param>
        /// <param name="baseCurrency">The base currency./></param>
        /// <param name="targetCurrency">The target currency./></param>
        /// <returns>The converted decimal amount.</returns>
        private static async Task<decimal> CalculateExchangeRateAsync(decimal amount, string baseCurrency, string targetCurrency)
        {
            var config = SettingsManager.GetSettings();

            using (var httpClient = new HttpClient())
            {
                // various API's were recomended, however I had a previous token with this API provider so used this provider instead.
                Uri uri = new Uri(string.Concat(config["FreeCurrencyAPIServiceUrl"], baseCurrency));

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                string result = await response.Content.ReadAsStringAsync();
                JObject rateData = JObject.Parse(result);
                decimal conversionRate = (decimal)(rateData["data"][targetCurrency]);

                // TODO : Get the currency decimal places, disregarded in this assessment.
                return Math.Round((amount * conversionRate), 2);
            }
        }

        #endregion
    }
}
