using BLL.Interfaces;
using CyberGooseReview.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace CyberGooseReview.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IProductService _productService;
        private IReviewService _reviewService;
        private IUserService _userService;

        public HomeController(ILogger<HomeController> logger, IReviewService reviewService, IUserService userService, IProductService productService)
        {
            _logger = logger;
            _reviewService = reviewService;
            _userService = userService;
            _productService = productService;
        }

        public IActionResult Index()
        {
            var Products = _productService.GetAllProducts().Select(p => new ProductModel()
            {
                Name = p.Name,
                Description = p.Description,
                ProductPicture = p.ProductPicture,
                Id = p.Id,
                UserRating = p.UserRating,
                Country = p.Country,
                CommonRating = p.CommonRating,
                CriticRating = p.CriticRating,
                Year = p.Year
            });
            return View(Products.Reverse());
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

        public IActionResult ChangeTheme()
        {
            var a = Request.Cookies["theme"];
            if (Request.Cookies["theme"] == null)
            {
                Response.Cookies.Append("theme", "dark");
            }
            else
            {
                if (Request.Cookies["theme"] == "dark")
                {
                    Response.Cookies.Append("theme", "light");
                }
                else if (Request.Cookies["theme"] == "light")
                {
                    Response.Cookies.Append("theme", "dark");
                }
            }
            string returnUrl = HttpContext.Request.Query["returnUrl"];
            if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }
    }
}