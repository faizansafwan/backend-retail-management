namespace retail_management.Models.Dto
{
    public class AddProductDto
    {
        public required string ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductCategory { get; set; }
        public string? SupplierName { get; set; }
    }
}
