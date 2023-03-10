using OrderService.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> : IRepository<T> where T : BaseEntity
    {
        Task<List<T>> GetAll();
        Task<List<T>> Get(Expression<Func<T, object>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> func = null, Expression<Func<T, object>>[] expressions = null);
        Task<List<T>> Get(Expression<Func<T, object>> filter = null, params Expression<Func<T, object>>[] expressions);
        Task<T> GetById(Guid id);
        Task<T> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes);
        Task<T> GetSingleAsync(Expression<Func<T, object>> includes, params Expression<Func<T, object>>[] expressions);
        Task<T> AddAsync(T entity);
        T Update(T entity);
    }
}
