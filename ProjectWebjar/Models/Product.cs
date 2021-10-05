﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectWebjar.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PicturePath { get; set; }
        public string Price { get; set; }
        public bool IsDeleted { get; set; }

    }
}
