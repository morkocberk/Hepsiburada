using Data.Abstract;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Data.Repository
{
    public interface IRepository<T> where T: IBaseEntity
    {
        public List<T> GetAll();
        public void Insert(T entity);
        List<T> GetList(Func<T, bool> filter = null);
    }
}
