using System.ComponentModel.DataAnnotations.Schema;

namespace retail_management.Models.Entities
{
    public class InvoiceStock
    {
        public int InvoiceId { get; set; }

        public int StockId { get; set; }

        [ForeignKey("InvoiceId")]
        public Invoice Invoice { get; set; } = null!;

        [ForeignKey("StockId")]
        public Stock Stock { get; set; } = null!;

    }
}
