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

    public class StockController : Controller
    {

        private readonly ApplicationDbContext dbContext;

        public StockController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // Reusable function to get the ShopId from token
        private int? GetShopIdFromToken()
        {
            var shopIdClaim = User.FindFirst("Id")?.Value;
            return shopIdClaim != null ? int.Parse(shopIdClaim) : null;
        }

        [Authorize]
        // GET: api/Stock/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Stock>> GetStockById(int id)
        {
            // Get ShopId from token
            var shopId = GetShopIdFromToken();
            if (shopId == null)
            {
                return Unauthorized(new { message = "Invalid Shop ID or Token" });
            }

            var stock = await dbContext.Stocks
                .Where(e => e.ShopId == shopId && e.Id == id)
                .ToListAsync();

            if (stock == null)
            {
                return NotFound(new { message = "Stock item not found" });
            }

            return Ok(stock);
        }


        [Authorize]
        // POST: api/Stock
        [HttpPost]
        public async Task<ActionResult<Stock>> AddStock(AddStockDto stockDto)
        {
            // Get ShopId from token
            var shopId = GetShopIdFromToken();
            if (shopId == null)
            {
                return Unauthorized(new { message = "Invalid Shop ID or Token" });
            }

            // Validate if the Product exists
            var product = await dbContext.Products
                .IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == stockDto.ProductId && p.ShopId == shopId);

            if (product == null)
            {
                return BadRequest(new { message = "Invalid Product ID" });
            }


            // Check if stock already exists for the given Product in the Shop
            var existingStock = await dbContext.Stocks
                .FirstOrDefaultAsync(s => s.Id == stockDto.ProductId && s.ShopId == shopId);

            int updatedCurrentStock = stockDto.StockAdjustment; // Default if no existing stock

            if (existingStock != null)
            {
                updatedCurrentStock = existingStock.CurrentStock + stockDto.StockAdjustment;
                existingStock.StockAdjustment = stockDto.StockAdjustment;
                existingStock.CurrentStock = updatedCurrentStock;
                existingStock.NewStock = updatedCurrentStock;
                existingStock.CostPrice = stockDto.CostPrice;
                existingStock.SellingPrice = stockDto.SellingPrice;
                existingStock.Total = existingStock.StockAdjustment * existingStock.CostPrice;

                await dbContext.SaveChangesAsync();
                return Ok(existingStock);
            }


            // If stock does not exist, create a new record
            var newStock = new Stock
            {
                ProductId = stockDto.ProductId,
                CostPrice = stockDto.CostPrice,
                SellingPrice = stockDto.SellingPrice,
                StockAdjustment = stockDto.StockAdjustment,
                CurrentStock = updatedCurrentStock,
                NewStock = updatedCurrentStock,
                ShopId = shopId.Value,
                Total = stockDto.StockAdjustment * stockDto.CostPrice // Calculate Total
            };

            dbContext.Stocks.Add(newStock);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStockById), new { id = newStock.Id }, newStock);
        }


        [Authorize]
        [HttpGet()]
        public async Task<ActionResult<Stock>> GetShopStock()
        {

            var shopId = GetShopIdFromToken();
            if (shopId == null)
            {
                return Unauthorized(new { message = "Invalid Shop ID or Token" });
            }

            var stocks = await dbContext.Stocks
                .Where(e => e.ShopId == shopId)
                .ToListAsync();

            if (stocks.Count == 0)
                return NotFound(new { message = "No products found for this shop" });

            return Ok(stocks);
        }


        [Authorize]
        // GET: api/Stock/{id}
        [HttpGet("stock/{id}")]
        public async Task<ActionResult<Stock>> GetStockByProduct(int id)
        {
            // Get ShopId from token
            var shopId = GetShopIdFromToken();
            if (shopId == null)
            {
                return Unauthorized(new { message = "Invalid Shop ID or Token" });
            }

            var stock = await dbContext.Stocks
                .Where(e => e.ShopId == shopId && e.ProductId == id)
                .ToListAsync();

            if (stock == null)
            {
                return NotFound(new { message = "Stock item not found" });
            }

            return Ok(stock);
        }


        [Authorize]
        [HttpPost("bulk")]
        public async Task<ActionResult> AddMultipleStock([FromBody] List<AddStockDto> stockDtos)
        {
            var shopId = GetShopIdFromToken();
            if (shopId == null)
            {
                return Unauthorized(new { message = "Invalid Shop ID or Token" });
            }

            var createdStocks = new List<Stock>();

            var groupedStockDtos = stockDtos
                .GroupBy(s => s.ProductId)
                .ToList();

            foreach (var group in groupedStockDtos)
            {
                var productId = group.Key;
                var product = await dbContext.Products
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(p => p.Id == productId && p.ShopId == shopId);

                if (product == null)
                {
                    continue; // Skip invalid product
                }

                var existingStock = await dbContext.Stocks
                    .FirstOrDefaultAsync(s => s.ProductId == productId && s.ShopId == shopId);



                int currentStock = existingStock?.NewStock ?? 0;
                int currentTotalValue = (int)(existingStock?.Total ?? 0);

                foreach (var stockDto in group)
                {
                    int stockAdjustment = stockDto.StockAdjustment;
                    

                    if (existingStock != null)
                    {
                        existingStock.StockAdjustment = stockAdjustment;
                        existingStock.CurrentStock = currentStock;
                        existingStock.NewStock += stockAdjustment;
                        existingStock.CostPrice = stockDto.CostPrice;
                        existingStock.SellingPrice = stockDto.SellingPrice;
                        existingStock.Total = (stockAdjustment * stockDto.CostPrice) + currentTotalValue ;

                        createdStocks.Add(existingStock);
                    }
                    else
                    {
                        var newStock = new Stock
                        {
                            ProductId = productId,
                            CostPrice = stockDto.CostPrice,
                            SellingPrice = stockDto.SellingPrice,
                            StockAdjustment = stockAdjustment,
                            CurrentStock = currentStock,
                            NewStock = currentStock + stockAdjustment,
                            ShopId = shopId.Value,
                            Total = stockAdjustment * stockDto.CostPrice
                        };

                        dbContext.Stocks.Add(newStock);
                        createdStocks.Add(newStock);

                        existingStock = newStock;
                    }
                }
            }

            await dbContext.SaveChangesAsync();
            return Ok(new { message = "Stocks processed successfully", stocks = createdStocks });
        }

    }
}
