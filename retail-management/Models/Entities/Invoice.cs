using System.ComponentModel.DataAnnotations.Schema;

namespace retail_management.Models.Entities
{
    public class Invoice
    {
        public int Id { get; set; }
        public required int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; } = null!;
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

        public List<InvoiceProduct> InvoiceProducts { get; set; } = new();
        public List<InvoiceStock> InvoiceStocks { get; set; } = new();

    }
}
