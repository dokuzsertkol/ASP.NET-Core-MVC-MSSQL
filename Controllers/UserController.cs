using Microsoft.AspNetCore.Mvc;
using htddict.Data;
using htddict.Models;
using Microsoft.EntityFrameworkCore;
using htddict.ViewModels;

namespace htddict.Controllers
{
    public class UserController : Controller
    {
        private readonly MyAppContext _context;

        public UserController(MyAppContext context) {  _context = context; }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
                var entries = await _context.Entries.Include(e => e.Author).Where(e => e.AuthorId == userId).OrderByDescending(e => e.Date).ToListAsync();
                ViewBag.Entries = entries;
                return View(user);
            }
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("index");
        }

        public async Task<IActionResult> Search(string u)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(uu => uu.Name == u);

            var currentUser = HttpContext.Session.GetInt32("UserId");
            if (currentUser != null && user?.Id == HttpContext.Session.GetInt32("UserId"))
            {
                return RedirectToAction("index");
            }

            TempData["SearchTerm"] = u;

            var entries = await _context.Entries.Include(e => e.Author).Where(e => e.Author != null && user != null && e.Author.Name == user.Name).OrderByDescending(e => e.Date).ToListAsync();
            ViewBag.Entries = entries;
            return View(user);
        }

        [HttpPost]
        public IActionResult Login(LoginUser loginUser)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Name == loginUser.Name);

                if (user != null)
                {
                    if (user.Restriction == "banned")
                    {
                        ModelState.AddModelError("Name", "User is banned.");
                        return View(loginUser);
                    }
                    else if (user.Password == loginUser.Password)
                    {
                        HttpContext.Session.SetInt32("UserId", user.Id);
                        HttpContext.Session.SetString("Username", user.Name);
                        HttpContext.Session.SetString("Role", user.Role);
                        return RedirectToAction("index");
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Wrong password.");
                        return View(loginUser);
                    }
                }
                else
                {
                    ModelState.AddModelError("Name", "User not found.");
                    return View(loginUser);
                }
            }

            return View(loginUser);
        }

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                bool usernameExists = _context.Users.Any(u => u.Name == user.Name);

                if (usernameExists)
                {
                    ModelState.AddModelError("Name", "Username is already taken.");
                    return View(user);
                }


                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("login");
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Ban(int id)
        {
            if (HttpContext.Session.GetString("Role") == "admin")
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                {
                    user.Restriction = "banned";
                    await _context.SaveChangesAsync();
                }
            }

            string referer = Request.Headers.Referer.ToString();
            return Redirect(referer ?? "/");
        }
        [HttpPost]
        public async Task<IActionResult> MakeAdmin(int id)
        {
            if (HttpContext.Session.GetString("Role") == "admin")
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                {
                    user.Role = "admin";
                    await _context.SaveChangesAsync();
                }
            }

            string referer = Request.Headers.Referer.ToString();
            return Redirect(referer ?? "/");
        }
        [HttpPost]
        public async Task<IActionResult> UnBan(int id)
        {
            if (HttpContext.Session.GetString("Role") == "admin")
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                {
                    user.Restriction = "none";
                    await _context.SaveChangesAsync();
                }
            }

            string referer = Request.Headers.Referer.ToString();
            return Redirect(referer ?? "/");
        }
        [HttpPost]
        public async Task<IActionResult> DeAdmin(int id)
        {
            if (HttpContext.Session.GetString("Role") == "admin")
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                {
                    user.Role = "user";
                    await _context.SaveChangesAsync();
                }
            }

            string referer = Request.Headers.Referer.ToString();
            return Redirect(referer ?? "/");
        }
    }
}
