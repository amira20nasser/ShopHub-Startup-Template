using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace myshop.DAL.Repos
{
    public interface IGenericRepo<TEntity, TKey> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAll(
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
            Expression<Func<TEntity, bool>>? filter = null);
        Task<TEntity?> GetById(TKey id);

        Task AddAsync(TEntity entity);

        void Remove(TEntity entity);
        void Update(TEntity entity);

    }
}
