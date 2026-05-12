using Microsoft.AspNetCore.Mvc;
using WatchList.Models;

namespace WatchList.Controllers
{
    public class WatchController : Controller
    {
        // Статический список вместо базы данных
        private static List<WatchItem> items = new List<WatchItem>
        {
            new WatchItem { Id = 1, Title = "Интерстеллар", Type = "Фильм", Status = "Не просмотрено" },
            new WatchItem { Id = 2, Title = "Во все тяжкие", Type = "Сериал", Status = "Не просмотрено" },
            new WatchItem { Id = 3, Title = "Дюна", Type = "Фильм", Status = "Просмотрено" }
        };

        // Главная страница - вывод по статусу
        public IActionResult Index()
        {
            return View(items);
        }

        // Пометить как просмотренное
        public IActionResult MarkAsWatched(int id)
        {
            var item = items.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                item.Status = "Просмотрено";
            }
            return RedirectToAction("Index");
        }

        // GET - форма добавления
        public IActionResult Add()
        {
            return View();
        }

        // POST - сохранить новый элемент
        [HttpPost]
        public IActionResult Add(WatchItem item)
        {
            item.Id = items.Count + 1;
            item.Status = "Не просмотрено";
            items.Add(item);
            return RedirectToAction("Index");
        }
    }
}