using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using CyberGooseReview.Models;
using Microsoft.AspNetCore.Mvc;

namespace CyberGooseReview.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IProductService _productService;
        private IReviewService _reviewService;
        private IUserService _userService;

        public UserController(ILogger<HomeController> logger, IReviewService reviewService, IUserService userService, IProductService productService)
        {
            _logger = logger;
            _reviewService = reviewService;
            _userService = userService;
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegistrationModel user)
        {
            if (ModelState.IsValid)
            {
                var config = new MapperConfiguration(cfg => cfg.CreateMap<UserRegistrationModel, UserDTO>());
                var mapper = new Mapper(config);
                var result = await _userService.CreateUser(mapper.Map<UserDTO>(user));
                if (result.Succeeded)
                {
                    await _userService.LogIn(mapper.Map<UserDTO>(user));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        if (!error.Description.Contains("Username"))
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    return View(user);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginModel user)
        {
            if (ModelState.IsValid)
            {
                var config = new MapperConfiguration(cfg => cfg.CreateMap<UserLoginModel, UserDTO>());
                var mapper = new Mapper(config);
                var result = await _userService.LogIn(mapper.Map<UserDTO>(user));
                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Wrong email or password");
                    return View(user);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _userService.LogOut();
            return RedirectToAction("Index", "Home");
        }
    }
}
