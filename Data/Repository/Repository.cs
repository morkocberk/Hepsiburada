using Data.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Data.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : IBaseEntity
    {
        private static List<TEntity> entityList = new List<TEntity>();

        public bool Any(Func<TEntity, bool> filter)
        {
            return entityList.Any(filter);
        }

        public TEntity Get(Func<TEntity, bool> filter)
        {
            return entityList.FirstOrDefault(filter);
        }

        public List<TEntity> GetAll()
        {
            return entityList;
        }

        public List<TEntity> GetList(Func<TEntity, bool> filter = null)
        {
            return entityList.Where(filter).ToList();
        }

        public void Insert(TEntity entity)
        {
            entityList.Add(entity);
        }

        public TEntity Update(Func<TEntity, bool> filter, TEntity entity)
        {
            var data = entityList.SingleOrDefault(filter);
            if (data != null)
            {
                entityList.Remove(data);
                entityList.Add(entity);
            }
            return entity;
        }

        public bool BulkUpdate(Func<TEntity, bool> filter, List<TEntity> entities)
        {
            var dataList = entityList.Where(filter).ToList();
            if (dataList != null && dataList.Count == entities.Count)
            {
                dataList.ForEach(data => entityList.Remove(data));
                entities.ForEach(entity => entityList.Add(entity));
            }
            else
                return false;
            return true;
        }
    }
}
