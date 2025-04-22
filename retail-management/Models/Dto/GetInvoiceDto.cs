namespace retail_management.Models.Dto
{
    public class GetInvoiceDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;

        public decimal Total { get; set; }
        public decimal Paid { get; set; }
        public decimal Balance { get; set; }
        public DateTime InvoiceDate { get; set; }

        public List<GetInvoiceProductDto> InvoiceProducts { get; set; } = new();
        public List<GetInvoiceStockDto> InvoiceStocks { get; set; } = new();
    }

    public class GetInvoiceProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal SubTotal { get; set; }
    }

    public class GetInvoiceStockDto
    {
        public int StockId { get; set; }
        public int Quantity { get; set; }
    }
}
