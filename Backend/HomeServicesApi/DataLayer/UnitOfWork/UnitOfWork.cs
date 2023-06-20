
using DataLayer.Entities;
using DataLayer.Repositories;

namespace DataLayer.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private Dictionary<Type, object> _repositories;

        public UnitOfWork(AppDbContext dbContext)
        {
            this._dbContext = dbContext;
            _repositories = new Dictionary<Type, object>();
        }

        public TRepository GetRepository<TRepository, TEntity>()
            where TRepository : RepositoryBase<TEntity>
            where TEntity : BaseEntity
        {
            if (_repositories.ContainsKey(typeof(TRepository)))
                return (TRepository)_repositories[typeof(TRepository)];

            var repository = Activator.CreateInstance(typeof(TRepository), new object[] { _dbContext }) as TRepository;
            _repositories.Add(typeof(TRepository), repository);
            return repository;
        }

        public void Commit()
        {
            try
            {
                _dbContext.SaveChanges();
            }
            catch (Exception exception)
            {
                var errorMessage = "Error when saving to the database: "
                    + $"{exception.Message}\n\n"
                    + $"{exception.InnerException}\n\n"
                    + $"{exception.StackTrace}\n\n";

                Console.WriteLine(errorMessage);
            }
        }

        public void Rollback()
        {
            // Implement rollback logic if necessary
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
