using System;
using System.Collections.Generic;
using System.Text;

namespace myshop.DAL.Repos
{
    public interface IGenericRepo<TEntity, TKey> where TEntity : class
    {
        Task<IReadOnlyList<TEntity>> GetAll();
        Task<TEntity?> GetById(TKey id);

        Task AddAsync(TEntity entity);

        void Remove(TEntity entity);
        void Update(TEntity entity);

    }
}
