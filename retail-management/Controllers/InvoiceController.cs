using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using retail_management.Data;
using retail_management.Models.Dto;
using retail_management.Models.Entities;

namespace retail_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public InvoiceController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // Reusable function to get the ShopId from token
        private int? GetShopIdFromToken()
        {
            var shopIdClaim = User.FindFirst("Id")?.Value;
            return shopIdClaim != null ? int.Parse(shopIdClaim) : null;
        }

        // GET: api/Invoice
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoices()
        {
            // Get ShopId from token
            var shopId = GetShopIdFromToken();
            if (shopId == null)
            {
                return Unauthorized(new { message = "Invalid Shop ID or Token" });
            }

            var invoice =  await dbContext.Invoices
                .Where(e => e.Customer.ShopId == shopId)
                .Include(i => i.Customer)
                .Include(i => i.InvoiceProducts)
                    .ThenInclude(ip => ip.Product)
                .Include(i => i.InvoiceStocks)
                    .ThenInclude(i => i.Stock)
                .ToListAsync();

            return Ok(invoice);
        }

        // GET: api/Invoice/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Invoice>> GetInvoice(int id)
        {
            // Get ShopId from token
            var shopId = GetShopIdFromToken();
            if (shopId == null)
            {
                return Unauthorized(new { message = "Invalid Shop ID or Token" });
            }

            var invoice = await dbContext.Invoices.Where(e => e.Customer.ShopId == shopId)
                .Include(i => i.Customer)
                .Include(i => i.InvoiceProducts)
                    .ThenInclude(ip => ip.Product)
                .Include(i => i.InvoiceStocks)
                    .ThenInclude(i => i.Stock)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
                return NotFound();

            return invoice;
        }


        // POST: api/Invoice
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Invoice>> CreateInvoice(AddInvoiceDto invoiceDto)
        {
            var shopId = GetShopIdFromToken();
            if (shopId == null)
            {
                return Unauthorized(new { message = "Invalid Shop ID or Token" });
            }

            // Validate Customer belongs to Shop
            var customer = await dbContext.Customers.FindAsync(invoiceDto.CustomerId);
            if (customer == null || customer.ShopId != shopId)
            {
                return BadRequest(new { message = "Invalid customer or customer does not belong to your shop" });
            }

            // Validate Products belong to Shop
            foreach (var ip in invoiceDto.Products)
            {
                var product = await dbContext.Products.FindAsync(ip.ProductId);
                if (product == null || product.ShopId != shopId)
                {
                    return BadRequest(new { message = $"Product {ip.ProductId} does not belong to your shop" });
                }
            }

            // Validate Stocks belong to Shop
            foreach (var stock in invoiceDto.Stocks)
            {
                var stockEntity = await dbContext.Stocks.FindAsync(stock.StockId);
                if (stockEntity == null || stockEntity.ShopId != shopId)
                {
                    return BadRequest(new { message = $"Stock {stock.StockId} does not belong to your shop" });
                }
            }

            // Create Invoice entity
            var invoice = new Invoice
            {
                CustomerId = invoiceDto.CustomerId,
                InvoiceDate = DateTime.UtcNow,
                InvoiceProducts = invoiceDto.Products.Select(p => new InvoiceProduct
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity,
                    SellingPrice = p.SellingPrice,
                    Discount = p.Discount,
                    SubTotal = p.SubTotal,
                    Total = p.Total,
                    Paid = p.Paid,
                    Balance = p.Balance
                }).ToList(),

                InvoiceStocks = invoiceDto.Stocks.Select(s => new InvoiceStock
                {
                    StockId = s.StockId,
                    Quantity = s.Quantity
                }).ToList()
            };

            dbContext.Invoices.Add(invoice);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInvoice), new { id = invoice.Id }, invoice);
        }
    }
}
