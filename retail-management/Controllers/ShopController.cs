using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using retail_management.Data;
using retail_management.Models.Dto;
using retail_management.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace retail_management.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        
           private readonly ApplicationDbContext dbContext;
           private readonly IConfiguration configuration;

           public ShopController(ApplicationDbContext dbContext, IConfiguration configuration)
           {
               this.dbContext = dbContext;
               this.configuration = configuration;
           }


           [HttpGet("shops")]
           public async Task<ActionResult<IEnumerable<Shop>>> GetShop()
           {
               var shopList = await dbContext.Shops.ToListAsync();

               return Ok(shopList);
           }

           [HttpPost("register")]
           public async Task<ActionResult<Shop>> CreateShop(AddShopDto shopDto)
           {
               var shop = new Shop()
               {
                   OwnerName = shopDto.OwnerName,
                   ShopName = shopDto.ShopName,
                   Phone = shopDto.Phone,
                   Email = shopDto.Email,
                   Address = shopDto.Address,
                   Username = shopDto.Username,
                   Password = shopDto.Password,
                   IsActive = shopDto.IsActive
               };
               dbContext.Shops.Add(shop);
               await dbContext.SaveChangesAsync();

               return Ok(shop);
           }


           [HttpPost("login")]
           public IActionResult Login(LoginDto loginDto)
           {
               if (loginDto == null)
               {
                   return BadRequest("Invalid request");
               }

               var user = dbContext.Shops.FirstOrDefault(x => x.Username == loginDto.Username && x.Password == loginDto.Password);


               if (user == null)
                   return Unauthorized("Invalid Credentials");


               var claims = new[]
               {
                   new Claim(JwtRegisteredClaimNames.Sub, configuration["Jwt:Subject"] ?? "default_subject"),
                   new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                   new Claim("Id", user.Id.ToString()),
                   new Claim("UserName", user.Username)
               };

               var key = configuration["Jwt:Key"];
               if (string.IsNullOrEmpty(key))
                   return StatusCode(500, "JWT Key is not configured");

               var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
               var signin = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
               var token = new JwtSecurityToken(
                       configuration["Jwt:Issuer"],
                       configuration["Jwt:Audience"],
                       claims,
                       expires: DateTime.UtcNow.AddDays(6),
                       signingCredentials: signin);


               string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
               return Ok(new { Token = tokenValue, User = user });
           }


           [Authorize]
           [HttpGet("{id}")]
           public async Task<ActionResult<Shop>> GetShopById(int id)
           {
               var shop = await dbContext.Shops.FindAsync(id);

               if (shop == null)
                   return NotFound(new { message = "User Not Found" });

               return Ok(shop);
           }


           [HttpPut("{id}")]
           public async Task<ActionResult<Shop>> UpdateShop(int id, Shop updatedShop)
           {

               var existingShop = await dbContext.Shops.FindAsync(id);

               existingShop.ShopName = updatedShop.ShopName;
               existingShop.OwnerName = updatedShop.OwnerName;
               existingShop.Phone = updatedShop.Phone;
               existingShop.Email = updatedShop.Email;
               existingShop.Address = updatedShop.Address;
               existingShop.Username = updatedShop.Username;
               existingShop.Password = updatedShop.Password;
               existingShop.IsActive = updatedShop.IsActive;


               try
               {
                   await dbContext.SaveChangesAsync();
                   return Ok(new { message = "Shop Updated Successfully", shop = existingShop });
               }
               catch (Exception ex)
               {
                   return StatusCode(500, new { message = "Error updating shop", error = ex.Message });
               }
           }

           [HttpDelete("{id}")]
           public async Task<ActionResult<Shop>> DeleteShop(int id, Shop shop)
           {
               var existingShop = await dbContext.Shops.FindAsync(id);

               if (existingShop != null)
               {
                   dbContext.Shops.Remove(existingShop);
               }

               else
               {
                   return NotFound(new { Message = "Shop Not Found" });
               }

               try
               {
                   await dbContext.SaveChangesAsync();
                   return Ok(new { message = "Shop deleted successfully" });
               }

               catch (Exception ex)
               {
                   return StatusCode(500, new { message = "Error deleting shop", error = ex.Message });
               }
           }


        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<Shop>> GetMyShop()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity == null)
                return Unauthorized("No identity found");

            var shopIdClaim = identity.FindFirst("Id");

            if (shopIdClaim == null)
                return Unauthorized("Shop ID not found in token");

            if (!int.TryParse(shopIdClaim.Value, out int shopId))
                return Unauthorized("Invalid Shop ID");

            var shop = await dbContext.Shops.FindAsync(shopId);

            if (shop == null)
                return NotFound("Shop not found");

            return Ok(shop);
        }

    }
}
