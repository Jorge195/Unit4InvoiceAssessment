using InvoiceAssessmentApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceAssessmentApi.Services
{
    public interface IInvoiceService
    {
        /// <summary>
        /// Get all invoices from the Invoices collection
        /// </summary>
        /// <returns>The collection of all invoices</returns>
        Task<List<Invoice>> GetInvoices();

        /// <summary>
        /// Gets an Invoice by its invoice id.
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="currency"></param>
        /// <returns>The indicated invoice.</returns>
        Task<Invoice> GetInvoice(int invoiceId, string currency);

        /// <summary>
        /// Insert a Invoice.
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns>The awaited Task</returns>
        Task CreateInvoice(Invoice invoice);

        /// <summary>
        /// Updates an existing Invoice.
        /// </summary>
        /// <param name="invoiceId">The invoice sequencial number.</param>
        /// <param name="invoice">The invoice to update.</param>
        /// <returns>The awaited Task</returns>
        Task UpdateInvoice(int invoiceId, Invoice invoice);
    }
}
