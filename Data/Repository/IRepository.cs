using Data.Abstract;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Data.Repository
{
    public interface IRepository<TEntity> where TEntity: IBaseEntity
    {
        public List<TEntity> GetAll();
        public void Insert(TEntity entity);
        List<TEntity> GetList(Func<TEntity, bool> filter = null);
        bool Any(Func<TEntity, bool> filter);
        TEntity Get(Func<TEntity, bool> filter);
        bool Update(Func<TEntity, bool> filter, TEntity entity);
        bool BulkUpdate(Func<TEntity, bool> filter, List<TEntity> entities);
    }
}
