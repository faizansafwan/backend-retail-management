namespace retail_management.Models.Dto
{
    public class AddInvoiceDto
    {
        public required int CustomerId { get; set; }

        public List<InvoiceProductDto> Products { get; set; } = new();
        public List<InvoiceStockDto> Stocks { get; set; } = new();
    }
}
