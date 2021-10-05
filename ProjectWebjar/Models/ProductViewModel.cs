using Microsoft.AspNetCore.Http;

namespace ProjectWebjar.Models
{
    public class ProductViewModel
    {
        public string Name { get; set; }
        public IFormFile Picture { get; set; }
        public string Price { get; set; }
        //public bool IsDeleted { get; set; }
    }
}