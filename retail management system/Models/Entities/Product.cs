using System.ComponentModel.DataAnnotations.Schema;

namespace retail_management_system.Models.Entities
{
    public class Product
    {
        public int Id { get; set; } // primary key
        public required string ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductCategory { get; set; }
        public string? SupplierName { get; set;}

        public int ShopId { get; set; } // Foreign key
        [ForeignKey("ShopId")]
        public Shop? Shop { get; set; } // Navigation Property


        public List<Stock> Stocks { get; set; } = new();
        public List<InvoiceProduct> InvoiceProducts { get; set; } = new();



    }
}
