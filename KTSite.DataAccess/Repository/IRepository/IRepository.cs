using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T :class
    {
        T Get(int id);
        T Get(long id);
        T Get(string id);
        IEnumerable<T> GetAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includePoperties = null
            );
        T GetFirstOrDefault(
           Expression<Func<T, bool>> filter = null,
           string includePoperties = null
           );
        void Add(T entity);
        void Remove(int id);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}
