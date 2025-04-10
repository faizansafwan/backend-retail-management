using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using retail_management_system.Data;
using retail_management_system.Models;
using retail_management_system.Models.Entities;

namespace retail_management_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public CustomerController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // Reusable function to get the ShopId from token
        private int? GetShopIdFromToken()
        {
            var shopIdClaim = User.FindFirst("Id")?.Value;
            return shopIdClaim != null ? int.Parse(shopIdClaim) : null;
        }

        [HttpPost("add-customer")]
        public async Task<ActionResult<Customer>> AddCustomer(AddCustomerDto customerDto)
        {
            var shopId = GetShopIdFromToken();

            if (shopId == null)
            {
                return Unauthorized(new { Message = "Invalid Shop ID or Token" });
            }

            var checkProductName = await dbContext.Customers
                .AnyAsync(c => c.ShopId == shopId && c.CustomerName == customerDto.CustomerName);

            if (checkProductName) {
                return BadRequest(new { message = "Customer name already exist" });
            }

            var lastCustomer = await dbContext.Customers
                .Where(c => c.ShopId == shopId)
                .OrderByDescending(p => p.CustomerId)
                .FirstOrDefaultAsync();

            int newProductId = (lastCustomer != null) ? lastCustomer.CustomerId + 1 : 100;

            var customer = new Customer()
            {
                CustomerId = newProductId,
                CustomerName = customerDto.CustomerName,
                Email = customerDto.Email,
                PhoneNumber = customerDto.PhoneNumber,
                Address = customerDto.Address,
                CreditLimit = customerDto.CreditLimit,
                OpeningBalance = customerDto.OpeningBalance,
                ShopId = shopId.Value

            };

            dbContext.Customers.Add(customer);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, customer);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {
            var shopId = GetShopIdFromToken();


            if (shopId == null)
            {
                return Unauthorized(new { Message = "Invalid Shop ID or Token" });
            }

            var customer = await dbContext.Customers
                .Where(p => p.ShopId == shopId && p.Id == id)
                .ToListAsync();
            if (customer == null)
            {
                return NotFound(new { message = "Customer not found" });
            }
            return Ok(customer);
        }

        [HttpGet("customer-list")]
        public async Task<ActionResult<Customer>> CustomerByShop()
        {
            var shopId = GetShopIdFromToken();

            if (shopId == null)
            {
                return Unauthorized(new { Message = "Invalid Shop ID or Token" });
            }

            var customers = await dbContext.Customers
                .Where(c => c.ShopId == shopId)
                .ToListAsync();

            if (customers.Count == 0)
                return NotFound(new { message = "No Customer found" });

            return Ok(customers);
        }
    }
}
