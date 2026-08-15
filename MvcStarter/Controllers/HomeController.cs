using Microsoft.AspNetCore.Mvc;
using MvcStarter.Models;
using System.Diagnostics;

namespace MvcStarter.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ShowId(int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("The id must be a whole number");
            }

            if (id == null)
            {
                return BadRequest("An id is required");
            }

            return Content($"id: {id}");
        }

        public IActionResult About(string? name)
        {
            string message = string.IsNullOrWhiteSpace(name)
                ? "This is the about page." 
                : $"Welcome, {name.Trim()}!";

            var model = new AboutViewModel(message);
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult SubmitName(NameInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(NameForm), input);
            }

            return RedirectToAction(nameof(About), new { name = input.Name!.Trim() });
        }

        [HttpGet]
        public IActionResult NameForm()
        {
            var model = new NameInputModel();
            return View(model);
        } 

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
