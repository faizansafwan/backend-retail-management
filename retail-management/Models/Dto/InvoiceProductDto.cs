namespace retail_management.Models.Dto
{
    public class InvoiceProductDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal Discount { get; set; }

    }
}
