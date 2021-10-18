using Microsoft.AspNetCore.Mvc;
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
        private readonly ProjectWebjarContext _context;

        public ProductController(ProjectWebjarContext context, IHostingEnvironment hostingEnv)
        {
            _hostingEnv = hostingEnv;
            _context = context;
        }

        #region Post

        [HttpPost] //Create Products
        public async Task<ActionResult> PostProduct([FromForm] ProductViewModel ProductVm )
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

                    List<Attribute> attributes = new List<Attribute>();
                    List<AttributeProduct> attributeProducts = new List<AttributeProduct>();
                    Product product = new Product();
                    product.Name = ProductVm.Name;
                    product.PicturePath = AddressPicture;
                    product.IsDeleted = false;

                    _context.Add(product);
                    await _context.SaveChangesAsync();

                    foreach (var itemAttribute in ProductVm.TitleAttribute)
                    {
                        attributes.Add(new Attribute()
                        {
                            Title = itemAttribute.Title,
                        });
                    }

                    _context.AddRange(attributes);
                    await _context.SaveChangesAsync();

                    foreach (var itemAttributeProduct in ProductVm.ValueAttributeProducts)
                    {
                        var LastAttributeId = attributes.Max(x => x.Id);
                        attributeProducts.Add(new AttributeProduct()
                        {
                            Value = itemAttributeProduct.Value,
                            Price = itemAttributeProduct.Price,
                            InStock = itemAttributeProduct.InStock,
                            ProductId=product.Id,
                            AttributeId = LastAttributeId, 
                        });
                    }
                    
                    _context.AddRange(attributeProducts);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
            }
            else
            {
                return BadRequest();
            }
        }
        #endregion
        #region Get




        [HttpGet] //Read Products
        public List<Product> GetProduct()
        {
            var product = _context.Products.Where(x => x.IsDeleted == false)
                .Select(x => new Product
                {
                    Id = x.Id,
                    Name = x.Name,
                    PicturePath = x.PicturePath,
                    Comments = x.Comments,
                    AttributesProducts = x.AttributesProducts,
                });
            return product.ToList();
        }

        [HttpGet("{id}")] // Read Products By Id 
        public List<Product> GetProductBy(int id)
        {
            var product = _context.Products.Where(x => x.IsDeleted == false &&  x.Id==id)
                .Select(x => new Product
                {
                    Id = x.Id,
                    Name = x.Name,
                    PicturePath = x.PicturePath,
                    Comments = x.Comments,
                    AttributesProducts = x.AttributesProducts,
                    
                });
            return product.ToList();
        }

        #endregion
        #region Put

        [HttpPut("{id}")] //Update Products
        public async Task<ActionResult> PutProduct(int id,[FromForm] ProductViewModel ProductVm )
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
                    
                    var product = _context.Products.Find(id);
                  
                    if (id != product.Id)
                    {
                        return BadRequest();
                    }
                    else
                    {
                        
                        product.Name = ProductVm.Name;
                        product.PicturePath = AddressPicture;
                        product.IsDeleted = false;
                        _context.Update(product);
                        await _context.SaveChangesAsync();


                        
                        List<Attribute> attributes = new List<Attribute>();
                        
                        foreach (var itemAttribute in ProductVm.TitleAttribute)
                        {
                            attributes.Add(new Attribute()
                            {
                                Title = itemAttribute.Title,
                            });
                        }

                        _context.AddRange(attributes);
                        await _context.SaveChangesAsync();

                        List<AttributeProduct> attributeProducts = new List<AttributeProduct>();
                        foreach (var itemAttributeProduct in ProductVm.ValueAttributeProducts)
                        {
                            var LastAttributeId = attributes.Max(x => x.Id);
                            attributeProducts.Add(new AttributeProduct()
                            {
                                Value = itemAttributeProduct.Value,
                                Price = itemAttributeProduct.Price,
                                InStock = itemAttributeProduct.InStock,
                                ProductId = product.Id,
                                AttributeId = LastAttributeId,
                            });
                            
                        }
                        
                        var a = _context.AttributeProducts.Where(u => u.ProductId == id).ToList();
   
                        _context.RemoveRange(a);
                        await _context.SaveChangesAsync();
                        _context.AddRange(attributeProducts);
                        await _context.SaveChangesAsync();
                        return Ok();  
                    }
                }
            }
            else
            {
                return BadRequest();
            }
        }
        #endregion
        #region Delete

        

       
        [HttpDelete("{id}")] //Delete products
        public ActionResult<Product> DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product.IsDeleted == true)
            {
                return NotFound();
            }

            product.IsDeleted = true;
            _context.SaveChanges();

            return NoContent();

        }
        #endregion
    }
}
