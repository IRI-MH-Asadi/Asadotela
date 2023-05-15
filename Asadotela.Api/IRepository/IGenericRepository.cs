using Microsoft.Identity.Client;
using System.Linq.Expressions;
using System.Net;

namespace Asadotela.Api.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>> expression = null,
            Func<IQueryable<T>,IOrderedQueryable<T>> orderBy = null,
            List<string> includes = null
            );
        Task<T> GetAsync(Expression<Func<T, bool>> expression,List<string> includes = null);
        Task InsertAsync(T entity);
        Task InsertRangeAsync(IEnumerable<T> entities);
        Task DeleteAsync(int id);
        void DeleteRange(IEnumerable<T> entities);
        void Update(T entity);
    }
}
