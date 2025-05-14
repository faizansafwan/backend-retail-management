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
                    StockId = i.StockId
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
                    StockId = i.StockId
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

            // Validate Customer and include tracking for update
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

            // Process products and automatically update stock
            foreach (var item in dto.InvoiceProducts)
            {
                // Find product and its stock
                var product = await dbContext.Products
                    .Include(p => p.Stocks)
                    .FirstOrDefaultAsync(p => p.Id == item.ProductId && p.ShopId == shopId);

                if (product == null)
                    return NotFound($"Product with ID {item.ProductId} not found for this shop.");

                // Get the stock record for this product in this shop
                var stock = product.Stocks.FirstOrDefault(s => s.ShopId == shopId);
                if (stock == null)
                    return NotFound($"Stock record for Product ID {item.ProductId} not found.");

                // Check stock availability
                if (stock.NewStock < item.Quantity)
                    return BadRequest($"Insufficient stock for {product.ProductName}. Available: {stock.NewStock}, Requested: {item.Quantity}");

                // Update stock
                stock.NewStock -= item.Quantity;
                stock.StockAdjustment = -item.Quantity;
                stock.Total = stock.NewStock * stock.CostPrice;
                dbContext.Stocks.Update(stock);

                // Calculate invoice line total
                var subTotal = (item.Quantity * item.SellingPrice) - (item.Discount * item.Quantity);
                total += subTotal;

                // Add to invoice products
                invoice.InvoiceProducts.Add(new InvoiceProduct
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    SellingPrice = item.SellingPrice,
                    Discount = item.Discount,
                    SubTotal = subTotal
                });
            }

            // Final invoice totals
            invoice.Total = total;
            invoice.Balance = total - dto.Paid;

            // Update customer's opening balance with the remaining balance
            // If balance is positive (customer owes money), add to opening balance
            // If balance is negative (customer overpaid), subtract from opening balance
            customer.OpeningBalance += (long) invoice.Balance;

            // Save changes in transaction
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                // Update customer record
                dbContext.Customers.Update(customer);

                // Add invoice
                dbContext.Invoices.Add(invoice);

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"An error occurred while creating the invoice: {ex.Message}");
            }

            return CreatedAtAction(nameof(GetInvoice), new { id = invoice.Id }, invoice);
        }


        [Authorize]
        [HttpGet("rfm-data")]
        public async Task<ActionResult<IEnumerable<CustomerRfmDto>>> GetCustomerRfmData()
        {
            var shopId = GetShopIdFromToken();
            if (shopId == null)
                return Unauthorized("Invalid shop token.");

            var rfmData = await dbContext.Invoices
                .Where(i => i.Customer.ShopId == shopId)
                .GroupBy(i => new { i.CustomerId, i.Customer.CustomerName })
                .Select(g => new CustomerRfmDto
                {
                    CustomerId = g.Key.CustomerId,
                    CustomerName = g.Key.CustomerName,
                    LastInvoiceDate = g.Max(i => i.InvoiceDate),
                    InvoiceCount = g.Count(),
                    TotalSpent = g.Sum(i => i.Total)
                })
                .ToListAsync();

            return Ok(rfmData);
        }



    }
}
