﻿using System.Linq.Expressions;

namespace HONATIMEPIECES.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task RemoveAsync(T entity);
        Task SaveChangesAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetQueryable();
        IQueryable<T> GetAll();
    }
}
