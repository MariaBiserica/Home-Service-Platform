using DataLayer.Entities;
using DataLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        TRepository GetRepository<TRepository, TEntity>() 
            where TRepository : RepositoryBase<TEntity>
            where TEntity: BaseEntity; //try implementing without a second variable type
        void Commit();
        void Rollback();
    }
}
