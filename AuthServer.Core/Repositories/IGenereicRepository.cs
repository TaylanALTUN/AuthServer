using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Repositories
{
    public interface IGenereicRepository<T> where T: class
    {
        Task<T> GetByIdAsync(int id);

        Task<IQueryable<T>> GetAllAsync();

        IQueryable<T> Where(Expression<Func<T, bool>> expression);

        Task AddAsync(T entity);

        void Update(T entity);

        void Remove(T entity);
    }
}
