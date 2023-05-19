using Asadotela.Api.Models;
using Microsoft.Identity.Client;
using System.Linq.Expressions;
using System.Net;
using Microsoft.EntityFrameworkCore.Query;
using X.PagedList;

namespace Asadotela.Api.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>> expression = null,
            Func<IQueryable<T>,IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null);
        Task<IPagedList<T>> GetAllAsync(
            RequestParams requestParams,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null
            );
        Task<T> GetAsync(Expression<Func<T, bool>> expression, Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null);
        Task InsertAsync(T entity);
        Task InsertRangeAsync(IEnumerable<T> entities);
        Task DeleteAsync(int id);
        void DeleteRange(IEnumerable<T> entities);
        void Update(T entity);
    }
}
