using myshop.DAL.Repos;
using System;
using System.Collections.Generic;
using System.Text;

namespace myshop.DAL
{
    interface IUnitOfWork
    {
        IGenericRepo<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity: class;
        Task<int> SaveChangesAsync();
    }
}
