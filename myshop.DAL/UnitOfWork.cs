using myshop.DAL.Repos;
using myshop.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace myshop.DAL
{
    public class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
    {
        private readonly Dictionary<Type, object> _repos = [];
        public IGenericRepo<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : class
        {
            var type = typeof(TEntity);
            if (_repos.TryGetValue(type, out var repo))
            {
                return (IGenericRepo<TEntity, TKey>)repo;
            }

            var genericRepo = new GenericRepo<TEntity, TKey>(dbContext);
            _repos[type] = genericRepo;
            return genericRepo;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await dbContext.SaveChangesAsync();
        }
    }
}
