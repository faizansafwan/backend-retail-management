using retail_management_system.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace retail_management_system.Models
{
    public class AddStockDto
    {
        public required int ProductId { get; set; }
        [ForeignKey("ProductId")]

        [Column(TypeName = "decimal(18,2)")]
        public required long CostPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public required decimal SellingPrice { get; set; }
        public required int StockAdjustment { get; set; }
      
    }
}
