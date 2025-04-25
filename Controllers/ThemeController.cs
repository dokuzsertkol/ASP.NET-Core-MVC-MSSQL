using Microsoft.AspNetCore.Mvc;

namespace htddict.Controllers
{
    public class ThemeController : Controller
    {
        [HttpPost]
        public IActionResult Toggle()
        {
            var theme = HttpContext.Session.GetString("Theme");
            if (theme == "light")
            HttpContext.Session.SetString("Theme", "dark");
            else if (theme == "dark") HttpContext.Session.SetString("Theme", "light");
            else HttpContext.Session.SetString("Theme", "dark");

            // Redirect back to where the user was
            string referer = Request.Headers.Referer.ToString();
            return Redirect(referer ?? "/");
        }
    }
}
