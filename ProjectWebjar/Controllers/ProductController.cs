using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using ProjectWebjar.Data;
using ProjectWebjar.Models;

namespace ProjectWebjar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnv;
        private readonly ProjectWebjarContext _db;

        public ProductController(ProjectWebjarContext db, IHostingEnvironment hostingEnv)
        {
            _hostingEnv = hostingEnv;
            _db = db;
        }

        [HttpPost] //Create Products
        public async Task<ActionResult> PostProduct([FromForm] ProductViewModel ProductVm)
        {
            if (ProductVm.Picture != null)
            {
                if (!Directory.Exists(_hostingEnv.WebRootPath + "\\uploads\\"))
                {
                    Directory.CreateDirectory(_hostingEnv.WebRootPath + "\\uploads\\");
                }
                using (FileStream filestream = System.IO.File.Create(_hostingEnv.WebRootPath + "\\uploads\\" + ProductVm.Picture.FileName))
                {
                    ProductVm.Picture.CopyTo(filestream);
                    filestream.Flush();
                    var AddressPicture =  "\\uploads\\" + ProductVm.Picture.FileName;


                    Product product = new Product();
                    product.Name = ProductVm.Name;
                    product.PicturePath = AddressPicture; 
                    product.Price = ProductVm.Price;
                    product.IsDeleted = false;
                    _db.Add(product);
                    await _db.SaveChangesAsync();
                    return Ok();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet] //Read Products
        public List<Product> GetProduct()
        {
            var product = _db.Products.Where(x => x.IsDeleted == false)
                .Select(x => new Product
                {
                    Id = x.Id,
                    Name = x.Name,
                    PicturePath = x.PicturePath,
                    Price = x.Price,
                });
            return product.ToList();
        }

        [HttpGet("{id}")] // Read Products By Id 
        public ActionResult<Product> GetProductBy(int id)
        {

            var product = _db.Products.Find(id);
            if (product.IsDeleted == true)
            {
                return NotFound();
            }
            return product;
        }

         
        [HttpPut("{id}")] //Update Products
        public async Task<ActionResult> Putroduct(int id,[FromForm] ProductViewModel ProductVm )
        {
            if (ProductVm.Picture != null)
            {
                if (!Directory.Exists(_hostingEnv.WebRootPath + "\\uploads\\"))
                {
                    Directory.CreateDirectory(_hostingEnv.WebRootPath + "\\uploads\\");
                }
                using (FileStream filestream = System.IO.File.Create(_hostingEnv.WebRootPath + "\\uploads\\" + ProductVm.Picture.FileName))
                {
                    ProductVm.Picture.CopyTo(filestream);
                    filestream.Flush();
                    var AddressPicture = "\\uploads\\" + ProductVm.Picture.FileName;
                    
                    var product = _db.Products.Find(id);
                    if (id != product.Id)
                    {
                        return BadRequest();
                    }
                    else
                    {
                        
                        product.Name = ProductVm.Name;
                        product.PicturePath = AddressPicture;
                        product.Price = ProductVm.Price;
                        product.IsDeleted = false;
                        _db.Update(product);
                        await _db.SaveChangesAsync();
                        return Ok();
                    }
                }
            }
            else
            {
                return BadRequest();
            }
        }

        
        [HttpDelete("{id}")] //Delete products
        public ActionResult<Product> DeleteProduct(int id)
        {
            var product = _db.Products.Find(id);
            if (product.IsDeleted == true)
            {
                return NotFound();
            }

            product.IsDeleted = true;
            _db.SaveChanges();

            return NoContent();
        }
    }
}
