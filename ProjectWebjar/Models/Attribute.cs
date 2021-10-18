using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectWebjar.Models
{
    public class Attribute
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public ICollection<AttributeProduct> AttributeProducts { get; set; }
    }
}