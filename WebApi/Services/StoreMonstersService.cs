namespace OppgaveUkeEnModul3.WebApi.Services;

using OppgaveUkeEnModul3.Core.Interfaces;

using OppgaveUkeEnModul3.Core;

public class StoreMonstersService(
    IStoreMonstersRepository repository, StoreMonsterBuilder builder) : IStoreMonstersService

{
    public async Task<List<StoreMonster>> GetAsync(
        bool? outOfStock,
        int page,
        int pageSize,
        string? sortBy)
    {
        if (page < 1)
        {
            throw new ArgumentException("Page must be greater than 0.");
        }
        if (pageSize < 1 || pageSize > 100)
        {
            throw new ArgumentException("PageSize must be between 1 and 100.");
        }

        return await repository.GetAsync(
            outOfStock,
            page,
            pageSize,
            sortBy
        );

    }
    public Task<StoreMonster?> GetAsync(Guid id)
    {
        return repository.GetAsync(id);
    }
    public async Task<StoreMonster> CreateAsync(CreateMonsterDTO dto)
    {
        var monster = builder.FromDto(dto);
        return await repository.AddAsync(monster);
    }
    public Task<bool> DeleteAsync(Guid id)
    {
        return repository.RemoveAsync(id);
    }
}
