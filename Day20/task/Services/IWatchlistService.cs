using WatchList.ViewModels;

namespace WatchList.Services
{
    public interface IWatchlistService
    {
        Task<List<WatchlistItemViewModel>> GetAllItemsAsync();
        Task<List<WatchlistItemViewModel>> GetItemsByStatusAsync(string status);
        Task<bool> AddItemAsync(WatchlistItemViewModel item);
        Task<bool> UpdateStatusAsync(int id, string newStatus);
        Task<bool> DeleteItemAsync(int id);
        Task<WatchlistItemViewModel?> GetItemByIdAsync(int id);
    }
}