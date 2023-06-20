using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Repositories
{
    public class RepositoryBase<T> where T : BaseEntity
    {
        private readonly AppDbContext context;
        private readonly DbSet<T> dbSet;

        public RepositoryBase(AppDbContext context)
        {
            context = context;
            dbSet = context.Set<T>();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            dbSet.Update(entity);
            return entity;
        }

        public void DeleteAsync(T entity)
        {
            dbSet.Remove(entity);

        }

        public IQueryable<T> Where(Func<T, bool> predicate)
        {
            return dbSet.Where(predicate).AsQueryable();
        }


    }
}
