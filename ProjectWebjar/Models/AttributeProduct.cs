using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectWebjar.Models
{
    public class AttributeProduct
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int AttributeId { get; set; }
        public Attribute Attribute { get; set; }
        public string Value { get; set; }
        public string Price { get; set; }
        public int InStock { get; set; }

    }
}
