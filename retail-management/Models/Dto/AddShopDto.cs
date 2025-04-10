namespace retail_management.Models.Dto
{
    public class AddShopDto
    {
        public required string ShopName { get; set; }
        public required string OwnerName { get; set; }
        public required string Phone { get; set; }
        public string? Email { get; set; }
        public required string Address { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public bool IsActive { get; set; }
    }
}
