using htddict.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using htddict.Models;
using System.Diagnostics;

namespace htddict.Controllers
{
    public class EntryController : Controller
    {
        private readonly MyAppContext _context;

        public EntryController(MyAppContext context) { _context = context; }

        public IActionResult Index()
        {
            var entries = _context.Entries.Include(e => e.Author).OrderByDescending(e => e.Date).ToList();

            return View(entries);
        }

        public ActionResult Entry(string title)
        {
            string decodedTitle = Uri.UnescapeDataString(title);
            var entry = _context.Entries.Include(e => e.Author).FirstOrDefault(e => e.Title == decodedTitle);
            entry ??= new Entry
                {
                    Title = title,
                    Content = "This entry does not exist."
                };
            return View(entry);
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("login", "user");
            }
            return View();
        }

        public async Task<IActionResult> Edit(int id, int? userId)
        {
            if (HttpContext.Session.GetString("Role") != "admin")
            {
                userId = HttpContext.Session.GetInt32("UserId");
            }
            if (userId == null)
            {
                return RedirectToAction("login", "user");
            }

            var entry = await _context.Entries
           .FirstOrDefaultAsync(e => e.Id == id && e.AuthorId == userId);

            if (entry != null) return View(entry);
            else return RedirectToAction("index");
        }

        public async Task<IActionResult> Search(string e)
        {
            TempData["SearchTerm"] = e;
            var entries = await _context.Entries.Include(ee => ee.Author).Where(ee => EF.Functions.Like(ee.Title, $"%{e}%")).OrderByDescending(ee => ee.Date).ToListAsync();
            return View(entries);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Entry entry)
        {
            if (ModelState.IsValid)
            {
                bool titleExists = await _context.Entries.AnyAsync(e => e.Title.ToLower().Trim() == entry.Title.ToLower().Trim());

                if (titleExists)
                {
                    ModelState.AddModelError("Title", "This title has been used before.");
                    return View(entry);
                }
                var userId = HttpContext.Session.GetInt32("UserId");

                if (userId == null)
                {
                    return RedirectToAction("logout", "user");
                }

                entry.AuthorId = userId.Value;
                _context.Add(entry);
                await _context.SaveChangesAsync();
                return RedirectToAction("index");
            }

            return View(entry);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id,int? userId)
        {
            userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("login", "user");
            }

            var entry = await _context.Entries
           .FirstOrDefaultAsync(e => e.Id == id && e.AuthorId == userId);

            if (entry != null)
            {
                _context.Entries.Remove(entry);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, int? userId, string content)
        {
            if (HttpContext.Session.GetString("Role") != "admin")
            {
                userId = HttpContext.Session.GetInt32("UserId");
            }
            if (userId == null)
            {
                return RedirectToAction("login", "user");
            }

            var entry = await _context.Entries
           .FirstOrDefaultAsync(e => e.Id == id && e.AuthorId == userId);

            if (entry != null)
            {
                entry.Content = content;
                await _context.SaveChangesAsync();
                return RedirectToAction("entry", new { title = Uri.EscapeDataString(entry.Title) } );
            }
            return RedirectToAction("entry");
        }
    }
}
