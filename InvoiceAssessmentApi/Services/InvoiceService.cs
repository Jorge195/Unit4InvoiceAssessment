using InvoiceAssessmentApi.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceAssessmentApi.Services
{
    public class InvoiceService : IInvoiceService
    {
        #region Members

        private readonly MongoClient mongoClient;
        private readonly IMongoDatabase database;
        private readonly IMongoCollection<Invoice> invoices;

        #endregion

        #region Public Functions

        public InvoiceService(
          MongoClient mClient,
          IConfiguration configuration)
        {
            mongoClient = mClient;
            database = mongoClient.GetDatabase(configuration["DatabaseName"]);
            invoices = database.GetCollection<Invoice>(configuration["CollectionName"]);
        }

        public async Task CreateInvoice(Invoice invoice)
        {
            await invoices.InsertOneAsync(invoice);
        }

        public async Task<Invoice> GetInvoice(int invoiceId, string currency)
        {
            var invoice = await invoices.FindAsync(p => p.InvoiceId == invoiceId);
            return invoice.FirstOrDefault();
        }

        public async Task<List<Invoice>> GetInvoices()
        {
            var allInvoices = await invoices.FindAsync(Invoice => true);
            return allInvoices.ToList();
        }

        public async Task UpdateInvoice(int invoiceId, Invoice InvoiceIn)
        {
            await invoices.ReplaceOneAsync(Invoice => Invoice.InvoiceId == invoiceId, InvoiceIn);
        }

        #endregion
    }
}
