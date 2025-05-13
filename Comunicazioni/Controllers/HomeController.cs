using System.Diagnostics;
using Comunicazioni.Models;
using Microsoft.AspNetCore.Mvc;

namespace Comunicazioni.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string r, string cod)
        {
            // Se "ruolo" è null o vuoto, l'utente non è autenticato
            if (String.IsNullOrEmpty(r))
            {
                // Reindirizza alla pagina di login
                return RedirectToAction("Login", "Login");
            }

            HttpContext.Session.SetString("r", r);
            HttpContext.Session.SetString("cod", cod);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
