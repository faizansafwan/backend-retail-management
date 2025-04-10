using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace retail_management.Models.Entities
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // primary key

        [Required]
        [StringLength(255)]
        public required string ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductCategory { get; set; }
        public string? SupplierName { get; set; }

        public int ShopId { get; set; } // Foreign key
        [ForeignKey("ShopId")]
        public Shop? Shop { get; set; } // Navigation Property

        [JsonIgnore]
        public List<Stock> Stocks { get; set; } = new();
        public List<InvoiceProduct> InvoiceProducts { get; set; } = new();
    }
}
