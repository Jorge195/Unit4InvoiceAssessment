using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InvoiceAssessmentApi.Models
{
    public class Invoice
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("invoiceId")]
        public int InvoiceId { get; set; }

        [BsonElement("supplier")]
        public string Supplier { get; set; }

        [BsonElement("dateIssued")]
        public string DateIssued { get; set; }

        [BsonElement("amount")]
        public decimal Amount { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("currency")]
        public string Currency { get; set; }


    }
}
