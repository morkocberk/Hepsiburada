using Data.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity
{
    public class BaseEntity : IBaseEntity
    {
        public string ProductCode { get; set; }
    }
}
