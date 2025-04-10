namespace retail_management_system.Models.Entities
{
    public class InvoiceStock
    {
        public int InvoiceId { get; set; }
        public int StockId { get; set; }

        public Invoice Invoice { get; set; } = null!;
        public Stock Stock { get; set; } = null!;

        public int Quantity { get; set; }
    }
}
