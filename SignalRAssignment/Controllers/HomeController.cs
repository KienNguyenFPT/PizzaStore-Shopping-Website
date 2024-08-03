using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalRAssignment.Models;
using SignalRAssignment.Services;
using System.Diagnostics;

namespace SignalRAssignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SalesManagementContext _context;
        private readonly SearchService _searchService;

        public HomeController(ILogger<HomeController> logger, SalesManagementContext context, SearchService searchService)
        {
            _logger = logger;
            _context = context;
            _searchService = searchService;
        }

        public async Task<IActionResult> Index(List<Product>? products = null)
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            return products != null ?
                        View(products) :
                        View(await _context.Products.ToListAsync());
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(string? str = "")
        {
            var products = await _searchService.SearchProHomeFunc(str);
            return View("Index", products);
        }
    }
}