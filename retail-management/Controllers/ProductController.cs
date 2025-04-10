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
    public class ProductController : Controller
    {

        private readonly ApplicationDbContext dbContext;

        public ProductController(ApplicationDbContext dbContext)
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
        [HttpPost()]
        public async Task<ActionResult<Product>> AddProduct(AddProductDto productDto)
        {
            var shopId = GetShopIdFromToken();

            if (shopId == null)
            {
                return Unauthorized(new { Message = "Invalid Shop ID or Token" });
            }

            // Check if the product name already exists in the same shop
            var existingProduct = await dbContext.Products
                .Where(p => p.ShopId == shopId && p.ProductName == productDto.ProductName)
                .FirstOrDefaultAsync();

            if (existingProduct != null)
            {
                return Conflict(new { message = "Product with the same name already exists in this shop." });
            }

            var product = new Product()
            {
                ProductName = productDto.ProductName,
                ProductDescription = productDto.ProductDescription,
                ProductCategory = productDto.ProductCategory,
                SupplierName = productDto.SupplierName,
                ShopId = shopId.Value,
            };

            dbContext.Products.Add(product);

            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            var shopId = GetShopIdFromToken();


            if (shopId == null)
            {
                return Unauthorized(new { Message = "Invalid Shop ID or Token" });
            }

            var product = await dbContext.Products
                .Where(p => p.ShopId == shopId && p.Id == id)
                .ToListAsync();
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }
            return Ok(product);
        }

        [HttpGet("product")]
        public async Task<ActionResult<Product>> GetProductByIdOrName(int? id, string? productName)
        {
            var shopId = GetShopIdFromToken();

            if (shopId == null)
                return Unauthorized(new { Message = "Invalid Shop ID or Token" });

            // Check if at least one parameter is provided
            if (id == null && string.IsNullOrEmpty(productName))
                return BadRequest(new { message = "Product ID or Product Name is required" });

            var product = await dbContext.Products
                .Where(p => p.ShopId == shopId && (p.Id == id || p.ProductName == productName))
                .FirstOrDefaultAsync();

            if (product == null)
                return NotFound(new { message = "Product not found" });

            return Ok(product);
        }

        //[Authorize]
        [HttpGet("product-list")]
        public async Task<ActionResult<IEnumerable<Product>>> ProductByShop()
        {
            var shopId = GetShopIdFromToken();

            if (shopId == null)
            {
                return Unauthorized(new { Message = "Invalid Shop ID or Token" });
            }

            var products = await dbContext.Products
                .Where(p => p.ShopId == shopId)
                .ToListAsync();

            if (products.Count == 0)
                return NotFound(new { message = "No products found for this shop" });

            return Ok(products);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> UpdateProduct(int id, Product UpdatedProduct)
        {
            var shopId = GetShopIdFromToken();

            if (shopId == null)
            {
                return Unauthorized(new { Message = "Invalid Shop ID or Token" });
            }

            var product = await dbContext.Products
                .Where(p => p.Id == id && p.ShopId == shopId).FirstOrDefaultAsync();

            product.ProductName = UpdatedProduct.ProductName;
            product.ProductDescription = UpdatedProduct.ProductDescription;
            product.ProductCategory = UpdatedProduct.ProductCategory;
            product.SupplierName = UpdatedProduct.SupplierName;

            try
            {
                await dbContext.SaveChangesAsync();
                return Ok(new { message = "Product Updated Successfully", products = product });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating product", error = ex.Message });
            }
        }


    }
}
