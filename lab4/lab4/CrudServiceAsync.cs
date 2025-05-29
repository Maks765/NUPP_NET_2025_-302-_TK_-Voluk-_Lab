using Transport.REST.Services;

public class CrudServiceAsync<T> : ICrudServiceAsync<T> where T : class
{
    private readonly IRepositoryAsync<T> _repo;

    public CrudServiceAsync(IRepositoryAsync<T> repo)
    {
        _repo = repo;
    }

    public async Task<bool> CreateAsync(T element) => await _repo.CreateAsync(element);
    public async Task<T> ReadAsync(Guid id) => await _repo.ReadAsync(id);
    public async Task<IEnumerable<T>> ReadAllAsync() => await _repo.ReadAllAsync();
    public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount) => await _repo.ReadAllAsync(page, amount);
    public async Task<bool> UpdateAsync(T element) => await _repo.UpdateAsync(element);
    public async Task<bool> RemoveAsync(T element) => await _repo.RemoveAsync(element);
    public async Task<bool> SaveAsync() => await _repo.SaveAsync();
}