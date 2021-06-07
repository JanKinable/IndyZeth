using Elbanique.IndyZeth.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Elbanique.IndyZeth.Infrastructure.File.Repositories
{
    public abstract class InMemoryRepositoryBase<T> : IRepository<T> where T : class
    {
        protected List<T> items = new List<T>();

        protected InMemoryRepositoryBase()
        {
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
        {
            var query = items.AsQueryable();
            query = query.Where(filter);
            var res = orderBy != null ? orderBy(query).ToList()
                                      : query.ToList();
            return res.ToList();
        }

        public IEnumerable<T> GetAll()
        {
            return new ReadOnlyCollection<T>(items);
        }

        public void Insert(T obj)
        {
            if (!items.Contains(obj)) items.Add(obj);
        }

        public void Update(T obj)
        {
            //Not applicable
            return;
        }

        public void Delete(T obj)
        {
            if (!items.Contains(obj)) return;
            items.Remove(obj);
        }

        public void DeleteAll()
        {
            items.Clear();
        }
    }
}
