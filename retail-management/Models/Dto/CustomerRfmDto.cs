namespace retail_management.Models.Dto
{
    public class CustomerRfmDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime LastInvoiceDate { get; set; }
        public int InvoiceCount { get; set; }
        public decimal TotalSpent { get; set; }
    }
}
