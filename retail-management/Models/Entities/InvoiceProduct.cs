using System.ComponentModel.DataAnnotations.Schema;

namespace retail_management.Models.Entities
{
    public class InvoiceProduct
    {
        public int InvoiceId { get; set; }

        public int ProductId { get; set; }

        [ForeignKey("InvoiceId")]
        public Invoice Invoice { get; set; } = null!;

        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;

        public required int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public required decimal SellingPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public required decimal Total { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public required decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Paid { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public required decimal Balance { get; set; }
    }
}
