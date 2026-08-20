using Microsoft.EntityFrameworkCore;
using myshop.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace myshop.DAL.Repos
{
    public class GenericRepo<TEntity, TKey>(ApplicationDbContext db) : IGenericRepo<TEntity, TKey> where TEntity : class
    {
        private readonly DbSet<TEntity> _dbSet = db.Set<TEntity>();
        public async Task AddAsync(TEntity entity)
        {
             await _dbSet.AddAsync(entity);
        }

        public async Task<IEnumerable<TEntity>> GetAll(
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null, 
            Expression<Func<TEntity, bool>>? filter = null)
        {
            IQueryable<TEntity> query = _dbSet;
            if (include != null)
                query = include(query);

            if (filter != null)
                query = query.Where(filter);

            return await query.ToListAsync();
        }

        public async Task<TEntity?> GetById(TKey id)
        {
            return await _dbSet.FindAsync(id);
        }

        public IQueryable<TEntity> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }
    }
}
