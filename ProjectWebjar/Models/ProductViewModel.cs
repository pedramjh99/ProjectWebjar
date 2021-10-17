using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace ProjectWebjar.Models
{
    public class ProductViewModel 
    {
        public string Name { get; set; }
        public IFormFile Picture { get; set; }
        public List<AttributeViewModel> TitleAttribute { get; set; }
        public List<AttributeProductViewModel> ValueAttributeProducts { get; set; }
    }
}