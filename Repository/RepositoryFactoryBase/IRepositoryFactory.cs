using Repository.RepositoryFactoryCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repository.RepositoryFactoryBase
{
    public interface IRepositoryFactory<TEntity> where TEntity : class
    { 
        void Insert(TEntity entity);
        void InsertRange(IEnumerable<TEntity> entities);        
        void Update(TEntity entity);
        void Delete(object id);
        void Delete(TEntity entity);
        TEntity Find(params object[] keyValues);
        IQueryable<TEntity> SelectQuery(string query, params object[] parameters);
        IQueryFluent<TEntity> Query(IQueryObject<TEntity> queryObject);
        IQueryFluent<TEntity> Query(Expression<Func<TEntity, bool>> query);
        IQueryFluent<TEntity> Query();
        IQueryable<TEntity> Queryable();       
    }
}
