using System.Text.Json;
using WatchList.Models;
using WatchList.Services;
using WatchList.ViewModels;

namespace WatchList.Services
{
    public class WatchlistService : IWatchlistService
    {
        private readonly string _dataFile = "watchlist_data.json";
        private readonly IWebHostEnvironment _environment;

        public WatchlistService(IWebHostEnvironment environment)
        {
            _environment = environment;
            _dataFile = Path.Combine(_environment.ContentRootPath, "watchlist_data.json");
        }

        private async Task<List<WatchItem>> LoadDataAsync()
        {
            if (!File.Exists(_dataFile))
                return new List<WatchItem>();

            var json = await File.ReadAllTextAsync(_dataFile);
            return JsonSerializer.Deserialize<List<WatchItem>>(json) ?? new List<WatchItem>();
        }

        private async Task SaveDataAsync(List<WatchItem> items)
        {
            var json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_dataFile, json);
        }

        public async Task<List<WatchlistItemViewModel>> GetAllItemsAsync()
        {
            var items = await LoadDataAsync();
            return items.Select(i => new WatchlistItemViewModel
            {
                Id = i.Id,
                Title = i.Title,
                Genre = i.Genre,
                Type = i.Type,
                Status = i.Status,
                Rating = i.Rating,
                DateAdded = i.DateAdded
            }).ToList();
        }

        public async Task<List<WatchlistItemViewModel>> GetItemsByStatusAsync(string status)
        {
            var items = await GetAllItemsAsync();
            return items.Where(i => i.Status == status).ToList();
        }

        public async Task<bool> AddItemAsync(WatchlistItemViewModel viewModel)
        {
            try
            {
                var items = await LoadDataAsync();

                var newItem = new WatchItem
                {
                    Id = items.Count > 0 ? items.Max(i => i.Id) + 1 : 1,
                    Title = viewModel.Title,
                    Genre = viewModel.Genre,
                    Type = viewModel.Type,
                    Status = viewModel.Status,
                    Rating = viewModel.Rating,
                    DateAdded = DateTime.Now
                };

                items.Add(newItem);
                await SaveDataAsync(items);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateStatusAsync(int id, string newStatus)
        {
            try
            {
                var items = await LoadDataAsync();
                var item = items.FirstOrDefault(i => i.Id == id);
                if (item != null)
                {
                    item.Status = newStatus;
                    await SaveDataAsync(items);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            try
            {
                var items = await LoadDataAsync();
                var item = items.FirstOrDefault(i => i.Id == id);
                if (item != null)
                {
                    items.Remove(item);
                    await SaveDataAsync(items);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<WatchlistItemViewModel?> GetItemByIdAsync(int id)
        {
            var items = await GetAllItemsAsync();
            return items.FirstOrDefault(i => i.Id == id);
        }
    }
}