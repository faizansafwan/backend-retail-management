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
        public async Task<ActionResult<IEnumerable<GetInvoiceDto>>> GetInvoices()
        {
            var shopId = GetShopIdFromToken();
            if (shopId == null)
                return Unauthorized(new { message = "Invalid Shop ID or Token" });

            var invoices = await dbContext.Invoices
                .Where(e => e.Customer.ShopId == shopId)
                .Include(i => i.Customer)
                .Include(i => i.InvoiceProducts)
                    .ThenInclude(ip => ip.Product)
                .Include(i => i.InvoiceStocks)
                    .ThenInclude(i => i.Stock)
                .ToListAsync();

            var result = invoices.Select(invoice => new GetInvoiceDto
            {
                Id = invoice.Id,
                CustomerId = invoice.CustomerId,
                CustomerName = invoice.Customer.CustomerName,
                Total = invoice.Total,
                Paid = invoice.Paid,
                Balance = invoice.Balance,
                InvoiceDate = invoice.InvoiceDate,
                InvoiceProducts = invoice.InvoiceProducts.Select(ip => new GetInvoiceProductDto
                {
                    ProductId = ip.ProductId,
                    ProductName = ip.Product.ProductName,
                    Quantity = ip.Quantity,
                    SellingPrice = ip.SellingPrice,
                    Discount = ip.Discount,
                    SubTotal = ip.SubTotal
                }).ToList(),
                InvoiceStocks = invoice.InvoiceStocks.Select(i => new GetInvoiceStockDto
                {
                    StockId = i.StockId,
                    Quantity = i.Quantity
                }).ToList()
            });

            return Ok(result);
        }


        // GET: api/Invoice/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<GetInvoiceDto>> GetInvoice(int id)
        {
            var shopId = GetShopIdFromToken();
            if (shopId == null)
                return Unauthorized(new { message = "Invalid Shop ID or Token" });

            var invoice = await dbContext.Invoices
                .Where(e => e.Customer.ShopId == shopId)
                .Include(i => i.Customer)
                .Include(i => i.InvoiceProducts)
                    .ThenInclude(ip => ip.Product)
                .Include(i => i.InvoiceStocks)
                    .ThenInclude(i => i.Stock)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
                return NotFound();

            var result = new GetInvoiceDto
            {
                Id = invoice.Id,
                CustomerId = invoice.CustomerId,
                CustomerName = invoice.Customer.CustomerName,
                Total = invoice.Total,
                Paid = invoice.Paid,
                Balance = invoice.Balance,
                InvoiceDate = invoice.InvoiceDate,
                InvoiceProducts = invoice.InvoiceProducts.Select(ip => new GetInvoiceProductDto
                {
                    ProductId = ip.ProductId,
                    ProductName = ip.Product.ProductName,
                    Quantity = ip.Quantity,
                    SellingPrice = ip.SellingPrice,
                    Discount = ip.Discount,
                    SubTotal = ip.SubTotal
                }).ToList(),
                InvoiceStocks = invoice.InvoiceStocks.Select(i => new GetInvoiceStockDto
                {
                    StockId = i.StockId,
                    Quantity = i.Quantity
                }).ToList()
            };

            return Ok(result);
        }



        // POST: api/Invoice
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateInvoice([FromBody] AddInvoiceDto dto)
        {
            var shopId = GetShopIdFromToken();
            if (shopId == null)
                return Unauthorized("Shop ID not found in token.");

            // Validate Customer
            var customer = await dbContext.Customers
                .FirstOrDefaultAsync(c => c.Id == dto.CustomerId && c.ShopId == shopId);
            if (customer == null)
                return NotFound($"Customer with ID {dto.CustomerId} not found for this shop.");

            decimal total = 0;

            var invoice = new Invoice
            {
                CustomerId = dto.CustomerId,
                Paid = dto.Paid,
                InvoiceDate = dto.InvoiceDate,
            };

            // InvoiceProducts
            foreach (var item in dto.InvoiceProducts)
            {
                var product = await dbContext.Products
                    .FirstOrDefaultAsync(p => p.Id == item.ProductId && p.ShopId == shopId);
                if (product == null)
                    return NotFound($"Product with ID {item.ProductId} not found for this shop.");

                var subTotal = (item.Quantity * item.SellingPrice) - item.Discount;
                total += subTotal;

                invoice.InvoiceProducts.Add(new InvoiceProduct
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    SellingPrice = item.SellingPrice,
                    Discount = item.Discount,
                    SubTotal = subTotal
                });
            }

            // InvoiceStocks
            foreach (var item in dto.InvoiceStocks)
            {
                var stock = await dbContext.Stocks
                    .FirstOrDefaultAsync(s => s.Id == item.StockId && s.ShopId == shopId);
                if (stock == null)
                    return NotFound($"Stock with ID {item.StockId} not found for this shop.");

                invoice.InvoiceStocks.Add(new InvoiceStock
                {
                    StockId = item.StockId,
                    Quantity = item.Quantity
                });
            }

            // Final totals
            invoice.Total = total;
            invoice.Balance = total - dto.Paid;

            // Save
            dbContext.Invoices.Add(invoice);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInvoice), new { id = invoice.Id }, invoice);
        }


    }
}
