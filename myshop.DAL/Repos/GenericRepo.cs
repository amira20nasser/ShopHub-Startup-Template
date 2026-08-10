using Microsoft.EntityFrameworkCore;
using myshop.DataAccess;
using System;
using System.Collections.Generic;
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

        public async Task<IReadOnlyList<TEntity>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity?> GetById(TKey id)
        {
            return await _dbSet.FindAsync(id);
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
