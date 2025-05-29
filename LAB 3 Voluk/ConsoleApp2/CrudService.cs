public class CrudServiceAsync<T> : ICrudServiceAsync<T> where T : class
{
    private readonly IRepository<T> _repository;
    private readonly TransportContext _context;

    public CrudServiceAsync(IRepository<T> repository, TransportContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<bool> CreateAsync(T element)
    {
        await _repository.AddAsync(element);
        return await SaveAsync();
    }

    public async Task<T> ReadAsync(Guid id)
    {
        return await _repository.GetByIdAsync((int)(object)id);
    }

    public async Task<IEnumerable<T>> ReadAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
    {
        return await _repository.GetAllAsync()
            .ContinueWith(t => t.Result.Skip((page - 1) * amount).Take(amount));
    }

    public async Task<bool> UpdateAsync(T element)
    {
        await _repository.Update(element);
        return await SaveAsync();
    }

    public async Task<bool> RemoveAsync(T element)
    {
        await _repository.Delete(element);
        return await SaveAsync();
    }

    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}