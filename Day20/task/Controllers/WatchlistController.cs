using Microsoft.AspNetCore.Mvc;
using WatchList.Services;
using WatchList.ViewModels;

namespace WatchList.Controllers
{
    public class WatchlistController : Controller
    {
        private readonly IWatchlistService _watchlistService;

        public WatchlistController(IWatchlistService watchlistService)
        {
            _watchlistService = watchlistService;
        }

        public async Task<IActionResult> Index()
        {
            var planToWatch = await _watchlistService.GetItemsByStatusAsync("PlanToWatch");
            var watching = await _watchlistService.GetItemsByStatusAsync("Watching");
            var watched = await _watchlistService.GetItemsByStatusAsync("Watched");

            ViewBag.PlanToWatch = planToWatch;
            ViewBag.Watching = watching;
            ViewBag.Watched = watched;

            return View();
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(WatchlistItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var success = await _watchlistService.AddItemAsync(model);

                if (success)
                {
                    TempData["SuccessMessage"] = $"✅ \"{model.Title}\" успешно добавлен в список!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Ошибка при добавлении записи");
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> MarkAsWatched(int id)
        {
            var success = await _watchlistService.UpdateStatusAsync(id, "Watched");

            if (success)
            {
                TempData["SuccessMessage"] = "🎉 Статус обновлен: просмотрено!";
            }
            else
            {
                TempData["ErrorMessage"] = "❌ Ошибка при обновлении статуса";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _watchlistService.GetItemByIdAsync(id);
            var title = item?.Title ?? "Запись";

            var success = await _watchlistService.DeleteItemAsync(id);

            if (success)
            {
                TempData["SuccessMessage"] = $"🗑 \"{title}\" удален из списка";
            }
            else
            {
                TempData["ErrorMessage"] = "❌ Ошибка при удалении";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}