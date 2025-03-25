using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using retail_management_system.Data;
using retail_management_system.Models;
using retail_management_system.Models.Entities;

namespace retail_management_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public ProductController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        //[HttpPost]
        //public async Task<ActionResult<Product>> AddProduct(AddProductDto productDto) {

        //{
          //      var product = new Product()
            //    {
              //      thi
                //};
            //dbContext.Products.Add()     
        //}
    }
}
