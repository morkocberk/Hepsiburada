using Data.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity
{
    public class Product : BaseEntity
    {
        public double Price { get; set; }
        public int Stock { get; set; }
    }
}
