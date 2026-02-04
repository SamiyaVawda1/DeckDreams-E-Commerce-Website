using System.Diagnostics;
using CloudPart3.Areas.Identity.Data;
using CloudPart3.Models;
using Microsoft.AspNetCore.Mvc;

namespace CloudPart3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _context = applicationDbContext;
        }

        public async Task<IActionResult> Index()
        {
            //fetching the product ids that i wanna display on the home page
            var productids = new List<int> { 4, 2, 8 };

            var selectedProducts = new List<Product>();
            foreach (var productid in productids)
            {
                var product = await _context.Product.FindAsync(productid);
                if (product != null)
                {
                    selectedProducts.Add(product);
                }
            }

            return View(selectedProducts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
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
