using System.ComponentModel.DataAnnotations.Schema;

namespace retail_management_system.Models.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public required string CustomerName { get; set; }
        
        public string? Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Address { get; set; }
        public long CreditLimit { get; set; }
        public long OpeningBalance { get; set; }

        public int ShopId { get; set; }
        [ForeignKey("ShopId")]

        public Shop? Shop { get; set; }

        public ICollection<Invoice> Invoices { get; } = new List<Invoice>();


    }
}
