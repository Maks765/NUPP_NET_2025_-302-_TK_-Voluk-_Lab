using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly TransportContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(TransportContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<T> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
    public Task Update(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }
    public Task Delete(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }
}