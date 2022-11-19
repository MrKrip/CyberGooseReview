﻿using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CyberGooseReview.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IProductService _productService;
        private IReviewService _reviewService;
        private IUserService _userService;

        public ProductController(ILogger<HomeController> logger, IReviewService reviewService, IUserService userService, IProductService productService)
        {
            _logger = logger;
            _reviewService = reviewService;
            _userService = userService;
            _productService = productService;
        }


        public IActionResult Search(string ProductName)
        {
            return View();
        }
    }
}