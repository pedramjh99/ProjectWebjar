using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using ProjectWebjar.Data;
using ProjectWebjar.Models;
using ProjectWebjar.Repository;
using Hangfire;

namespace ProjectWebjar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnv;
        private readonly ProjectWebjarContext _context;
        private readonly IRecurringJobManager _recurringJobManager;

        public ProductController(ProjectWebjarContext context, IHostingEnvironment hostingEnv, IRecurringJobManager recurringJobManager)
        {
            _hostingEnv = hostingEnv;
            _context = context;
            _recurringJobManager = recurringJobManager;
        }

        #region Post

        [HttpPost] //Create Products
        public async Task<ActionResult> PostProduct([FromForm] ProductViewModel ProductVm)
        {
            //------------------------------------Picture------------------------------\\
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
                    
                    
                    //var sizepic = filestream.Length;
                    //if(sizepic>100)
                    //{
                    //    _recurringJobManager.AddOrUpdate("test", () => System.IO.File.Delete(AddressPicture), Cron.Minutely);
                        
                    //}

                    //-------------------------------Add Product With Picture----------------------------\\
                    Product product = new Product();
                    product.Name = ProductVm.Name;
                    product.PicturePath = AddressPicture;
                    product.IsDeleted = false;
                    _context.Add(product);
                    await _context.SaveChangesAsync();

                    //-------------------------------Add attribute for Product----------------------------\\
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

                    //------------------------Add Value's and Price of attribute for Product-------------------\\
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
            CommentRepository comment = new CommentRepository();
            var products = _context.Products.Where(x => x.IsDeleted == false)
                .Select(x => new Product
                {
                    Id = x.Id,
                    Name = x.Name,
                    PicturePath = x.PicturePath,
                    AttributesProducts = x.AttributesProducts,
                }).ToList();

            foreach (var product in products)
            {
                product.Comments = comment.GetList(product.Id);
            }
            return products;
               
        }

        [HttpGet("{id}")] // Read Products By Id 
        public List<Product> GetProductBy(int id)
        {
            CommentRepository comment = new CommentRepository();

            var products = _context.Products.Where(x => x.IsDeleted == false && x.Id == id)
                .Select(x => new Product
                {
                    Id = x.Id,
                    Name = x.Name,
                    PicturePath = x.PicturePath,
                    AttributesProducts = x.AttributesProducts,

                }).ToList();
            foreach (var product in products)
            {
                product.Comments = comment.GetList(product.Id);
            }

            return products;
        }

        #endregion
        #region Put

        [HttpPut("{id}")] //Update Products
        public async Task<ActionResult> PutProduct(int id, [FromForm] ProductViewModel ProductVm)
        {
            //----------------------------------Picture------------------------------\\
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

                    //----------------------------Find And Update Product----------------------------\\
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
                                                    
                        //-------------------------Add New Attribute for Product----------------------------\\
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

                        //-------------------Find valu's and Price of Attribute and replace it---------------------\\
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

            return Ok();
        }
        #endregion

    }
}