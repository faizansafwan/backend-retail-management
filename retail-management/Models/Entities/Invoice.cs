using System.ComponentModel.DataAnnotations.Schema;

namespace retail_management.Models.Entities
{
    public class Invoice
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Paid { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }

        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

        public List<InvoiceProduct> InvoiceProducts { get; set; } = new();
        public List<InvoiceStock> InvoiceStocks { get; set; } = new();
    }
}
