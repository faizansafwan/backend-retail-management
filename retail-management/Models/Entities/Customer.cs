using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace retail_management.Models.Entities
{
    public class Customer
    {
        
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public required string CustomerName { get; set; }

        public string? Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Address { get; set; }
        public long CreditLimit { get; set; }
        public long OpeningBalance { get; set; }

        public int ShopId { get; set; }
        [ForeignKey("ShopId")]

        public Shop? Shop { get; set; }

        [JsonIgnore]
        public ICollection<Invoice> Invoices { get; } = new List<Invoice>();
    }
}
