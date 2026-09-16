using OppgaveUkeEnModul3.Core;

namespace OppgaveUkeEnModul3.Core.Interfaces;

public interface IStoreMonstersRepository
{
    StoreMonster Add(StoreMonster monster);
    Task<StoreMonster> AddAsync(StoreMonster monster);

    bool Remove(Guid id);
    Task<bool> RemoveAsync(Guid id);
    IEnumerable<StoreMonster> Get();


    Task<List<StoreMonster>> GetAsync(
        bool? outOfStock,
        int page,
        int pageSize,
        string? sortBy);
    StoreMonster? Get(Guid id);
    Task<StoreMonster?> GetAsync(Guid id);

}