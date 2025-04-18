namespace retail_management.Models.Dto
{
    public class ProductWithStockDto
    {
            public int ProductId { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public string? Category { get; set; }
            public string? Supplier { get; set; }
            public int TotalStock { get; set; }
            public decimal CostPrice { get; set; } 
            public decimal SellingPrice { get; set; }
            public decimal Total { get; set; }

    }
}
