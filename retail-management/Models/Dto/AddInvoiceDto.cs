namespace retail_management.Models.Dto
{
    public class AddInvoiceDto
    {
        public int CustomerId { get; set; }
        public decimal Paid { get; set; }
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
        public List<InvoiceProductDto> InvoiceProducts { get; set; } = new();
        public List<InvoiceStockDto> InvoiceStocks { get; set; } = new();
    }
}
