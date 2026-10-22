namespace DungeonsAndDragqueens.WebApi.Services;

using DungeonsAndDragqueens.Core;


public interface IStoreMonstersService
{
    Task<List<StoreMonster>> GetAsync(
        bool? outOfStock,
        int page,
        int pageSize,
        string? sortBy);

    Task<StoreMonster?> GetAsync(Guid id);
    Task<StoreMonster> CreateAsync(CreateMonsterDTO dto);
    Task<bool> DeleteAsync(Guid id);
}