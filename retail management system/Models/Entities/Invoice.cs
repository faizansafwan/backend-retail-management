using System.ComponentModel.DataAnnotations.Schema;

namespace retail_management_system.Models.Entities
{
    public class Invoice
    {
        public int Id { get; set; }

        public required int CustomerId { get; set; }
       
        public Customer Customer { get; set; } = null!;

        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

        //public required int ProductId { get; set; }
        //[ForeignKey("ProductId")]
        //public Product Product { get; set; } = null!;

        

        public List<InvoiceProduct> InvoiceProducts { get; set; } = new();
        public List<InvoiceStock> InvoiceStocks { get; set; } = new();


    }
}
