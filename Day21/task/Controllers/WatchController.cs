using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatchList.Data;
using WatchList.Models;

namespace WatchList.Controllers
{
    public class WatchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WatchController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Показать все
        public async Task<IActionResult> Index()
        {
            return View(await _context.WatchItems.ToListAsync());
        }

        // Добавить (GET)
        public IActionResult Create()
        {
            return View();
        }

        // Добавить (POST)
        [HttpPost]
        public async Task<IActionResult> Create(WatchItem item)
        {
            _context.WatchItems.Add(item);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Изменить статус
        public async Task<IActionResult> ChangeStatus(int id, string status)
        {
            var item = await _context.WatchItems.FindAsync(id);
            if (item != null)
            {
                item.Status = status;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}