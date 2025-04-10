using System.ComponentModel.DataAnnotations.Schema;

namespace retail_management.Models.Entities
{
    public class Stock
    {
        public int Id { get; set; }

        public required int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public required long CostPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public required decimal SellingPrice { get; set; }
        public int CurrentStock { get; set; } = 0;
        public required int StockAdjustment { get; set; }

        public int NewStock { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public int ShopId { get; set; } // Foreign key
        [ForeignKey("ShopId")]
        public Shop? Shop { get; set; } // Navigation Property
        public List<InvoiceStock> InvoiceStocks { get; set; } = new();
    }
}
