using Microsoft.EntityFrameworkCore;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.SeedWork;
using OrderService.Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private OrderDbContext _dbContext;

        public GenericRepository(OrderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IUnitOfWork UnitOfWork { get; }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            return entity;
        }

        public virtual async Task<List<T>> Get(Expression<Func<T, object>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> func = null, Expression<Func<T, object>>[] expressions = null)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            foreach (var item in expressions)
            {
                query = query.Include(item);
            }

            if(filter != null)
            {
                query = query.Where(filter);
            }

            if(func != null)
            {
                query = func(query);
            }

            return await query.ToListAsync();
        }

        public virtual Task<List<T>> Get(Expression<Func<T, object>> filter = null, params Expression<Func<T, object>>[] expressions)
        {
            return Get(filter, null, expressions);
        }

        public virtual async Task<List<T>> GetAll()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public virtual async Task<T> GetById(Guid id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public virtual async Task<T> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            foreach(var item in includes)
            {
                query=query.Include(item);
            }

            return await query.FirstOrDefaultAsync(i => i.Id == id);
        }

        public virtual async Task<T> GetSingleAsync(Expression<Func<T, object>> includes, params Expression<Func<T, object>>[] expressions)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            foreach (var item in expressions)
            {
                query = query.Include(item);
            }

            return await query.Where(includes).SingleOrDefaultAsync();
        }

        public virtual T Update(T entity)
        {
            _dbContext.Set<T>().Update(entity);
            return entity;
        }
    }
}
