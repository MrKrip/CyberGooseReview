using BLL.Interfaces;
using CyberGooseReview.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CyberGooseReview.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IProductService _productService;
        private IReviewService _reviewService;
        private IUserService _userService;

        public HomeController(ILogger<HomeController> logger, IReviewService reviewService, IUserService userService)
        {
            _logger = logger;
            _reviewService = reviewService;
            _userService = userService;
        }

        public IActionResult Index()
        {
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