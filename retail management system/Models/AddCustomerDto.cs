namespace retail_management_system.Models
{
    public class AddCustomerDto
    {
        public required string CustomerName { get; set; }
        public string? Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Address { get; set; }
        public long CreditLimit { get; set; }
        public long OpeningBalance { get; set; }

    }
}
