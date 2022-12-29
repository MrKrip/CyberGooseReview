using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using CyberGooseReview.Models;
using DAL.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CyberGooseReview.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IProductService _productService;
        private IReviewService _reviewService;
        private IUserService _userService;
        private int pageSize = 10;

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

        public IActionResult Profile(string id)
        {
            UserDataModel user = new UserDataModel(id, _userService);
            user.Roles = _userService.GetUserRoles(id).Result;
            user.ReviewCount = _reviewService.GetAllUserReview(id).Count();
            var reviews = _reviewService.GetAllUserReview(id).Reverse().Take(15).ToList().Select(r => new ReviewModel()
            {
                CreationDate = r.CreationDate,
                Details = r.ReviewDetails,
                DisLikes = r.DisLikes,
                Likes = r.Likes,
                userData = user,
                ProductId = r.ProductId,
                ProductName = _productService.GetProduct(r.ProductId).Name,
                Rating = r.Rating
            }).ToList();
            ItemWithReviewModel<UserDataModel> model = new ItemWithReviewModel<UserDataModel>()
            {
                Item = user,
                Reviews = reviews
            };
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Manage()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<UserDTO, UserManageModel>());
            var mapper = new Mapper(config);
            return View(mapper.Map<UserManageModel>(_userService.GetCurrentUser(User)));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Manage(UserManageModel model)
        {
            if (Request.Form.Files.Count > 0)
            {
                IFormFile file = Request.Form.Files.FirstOrDefault();
                using (var dataStream = new MemoryStream())
                {
                    await file.CopyToAsync(dataStream);
                    model.ProfilePicture = dataStream.ToArray();
                }
            }
            if (ModelState.IsValid)
            {
                var config = new MapperConfiguration(cfg => cfg.CreateMap<UserManageModel, UserDTO>());
                var mapper = new Mapper(config);
                var result = await _userService.UpdateUser(mapper.Map<UserDTO>(model), User);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        if (!error.Description.Contains("Username"))
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    return View(model);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Delete()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ConfirmDelete()
        {
            var result = await _userService.DeleteUser(User);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    if (!error.Description.Contains("Username"))
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                return View();
            }
            await _userService.LogOut();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public IActionResult NewRewiew(int Rating, string Review, int ProdId)
        {
            _reviewService.CreateReview(new ReviewDTO() { ProductId = ProdId, Rating = Rating, ReviewDetails = Review, UserId = _userService.GetCurrentUser(User).Id, User = _userService.GetCurrentUser(User) });
            return RedirectToAction("Product", "Product", new { id = ProdId });
        }

        [Authorize]
        [HttpPost]
        public IActionResult EditRewiew(int Rating, string Review, int ProdId)
        {
            var user = _userService.GetCurrentUser(User);
            var review = _reviewService.FindUserReviews(r => r.ProductId == ProdId && r.UserId == user.Id).FirstOrDefault();
            if (review != null)
            {
                review.Rating = Rating;
                review.ReviewDetails = Review;
                _reviewService.DeleteReview(review.Id);
                _reviewService.CreateReview(review);
            }
            return RedirectToAction("Product", "Product", new { id = ProdId });
        }

        [Authorize]
        public IActionResult DeleteRewiew(int ProdId)
        {
            var user = _userService.GetCurrentUser(User);
            var review = _reviewService.FindUserReviews(r => r.ProductId == ProdId && r.UserId == user.Id);
            if (review != null)
            {
                _reviewService.DeleteReview(review.FirstOrDefault().Id);
            }
            return RedirectToAction("Product", "Product", new { id = ProdId });
        }

        public async Task<int> Like(int id)
        {
            return 0;
        }

        public async Task<int> DisLike(int id)
        {
            return 0;
        }

        public IActionResult Reviews(string UserId, int page = 1)
        {
            ViewBag.UserId = UserId;
            var reviews = _reviewService.GetAllUserReview(UserId).Reverse().ToList().Select(r => new ReviewModel()
            {
                CreationDate = r.CreationDate,
                Details = r.ReviewDetails,
                DisLikes = r.DisLikes,
                Likes = r.Likes,
                userData = new UserDataModel(UserId, _userService),
                ProductId = r.ProductId,
                ProductName = _productService.GetProduct(r.ProductId).Name,
                Rating = r.Rating
            }).ToList();
            var count = reviews.Count;
            reviews = reviews.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            PageModel pageModel = new PageModel(count, page, pageSize);
            ItemsPageModel<ReviewModel> model = new ItemsPageModel<ReviewModel>(reviews, pageModel);
            return View(model);
        }
    }
}
