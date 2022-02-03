using Data.Entity;
using Data.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Abstract
{
    public interface IProductDataDal : IRepository<Product>
    {
    }
}
