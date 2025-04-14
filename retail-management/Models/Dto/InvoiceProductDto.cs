namespace retail_management.Models.Dto
{
    public class InvoiceProductDto
    {
        public required int ProductId { get; set; }
        public required int Quantity { get; set; }
        public required decimal SellingPrice { get; set; }
        public decimal Discount { get; set; }
        public required decimal SubTotal { get; set; }
        public required decimal Total { get; set; }
        public decimal Paid { get; set; }
        public required decimal Balance { get; set; }
    }
}
