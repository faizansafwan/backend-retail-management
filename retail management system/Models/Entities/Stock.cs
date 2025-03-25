using System.ComponentModel.DataAnnotations.Schema;

namespace retail_management_system.Models.Entities
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
        public required int CurrentStock { get; set; }
        public required int StockAdjustment {  get; set; }
        
        public int NewStock { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total {  get; set; }

        

        public List<InvoiceStock> InvoiceStocks { get; set; } = new();

        

    }
}
