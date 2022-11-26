using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using CyberGooseReview.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CyberGooseReview.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IProductService _productService;
        private IReviewService _reviewService;
        private IUserService _userService;
        private int pageSize = 10;

        public AdminController(ILogger<HomeController> logger, IReviewService reviewService, IUserService userService, IProductService productService)
        {
            _logger = logger;
            _reviewService = reviewService;
            _userService = userService;
            _productService = productService;
        }


        [HttpGet]
        public IActionResult Roles(string? roleName, int page = 1)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<IdentityRole, RoleModel>());
            var mapper = new Mapper(config);
            if (roleName == null)
            {
                var roles = _userService.GetAllRoles().Select(r => mapper.Map<RoleModel>(r));
                var count = roles.Count();
                roles = roles.Skip((page - 1) * pageSize).Take(pageSize);
                PageModel pageModel = new PageModel(count, page, pageSize);
                ItemsPageModel<RoleModel> model = new ItemsPageModel<RoleModel>(roles, pageModel);
                return View(model);
            }
            else
            {
                var roles = _userService.FindRole(r => r.Name.ToLower().Contains(roleName.ToLower())).Select(r => mapper.Map<RoleModel>(r));
                if (roles.Any())
                {
                    ViewBag.roleName = roleName;
                    var count = roles.Count();
                    roles = roles.Skip((page - 1) * pageSize).Take(pageSize);
                    PageModel pageModel = new PageModel(count, page, pageSize);
                    ItemsPageModel<RoleModel> model = new ItemsPageModel<RoleModel>(roles, pageModel);
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Role \"{roleName}\" does not exist");
                    roles = _userService.GetAllRoles().Select(r => mapper.Map<RoleModel>(r));
                    var count = roles.Count();
                    roles = roles.Skip((page - 1) * pageSize).Take(pageSize);
                    PageModel pageModel = new PageModel(count, page, pageSize);
                    ItemsPageModel<RoleModel> model = new ItemsPageModel<RoleModel>(roles, pageModel);
                    return View(model);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> NewRole(string roleName)
        {
            var result = await _userService.CreateNewRole(roleName);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    if (!error.Description.Contains("Username"))
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return RedirectToAction("Roles");
        }

        public async Task<IActionResult> DeleteRole(string id)
        {
            var result = await _userService.DeleteRole(id);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    if (!error.Description.Contains("Username"))
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return RedirectToAction("Roles");
        }

        [HttpGet]
        public async Task<IActionResult> UserRoles(string? userName, int page = 1)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<UserDataDTO, UserRolesModel>());
            var mapper = new Mapper(config);
            List<UserRolesModel> Users = new List<UserRolesModel>();
            if (userName == null)
            {
                Users = _userService.GetUsers().Select(u => mapper.Map<UserRolesModel>(u)).ToList();
            }
            else
            {
                ViewBag.userName = userName;
                Users = _userService.FindUsers(u => u.UserNick.ToLower().Contains(userName.ToLower()) || u.Email.ToLower().Contains(userName.ToLower()))
                    .Select(u => mapper.Map<UserRolesModel>(u)).ToList();
            }
            var count = Users.Count();
            Users = Users.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            for (int i = 0; i < Users.Count; i++)
            {
                Users[i].Roles = (await _userService.GetUserRoles(Users[i].Id)).ToList();
            }
            PageModel pageModel = new PageModel(count, page, pageSize);
            ItemsPageModel<UserRolesModel> itemsPageModel = new ItemsPageModel<UserRolesModel>(Users, pageModel);
            return View(itemsPageModel);
        }

        public async Task<IActionResult> ManageRoles(string id)
        {
            ViewBag.Id = id;
            ViewBag.UserName = _userService.GetUserById(id).UserNick;
            var roles = _userService.GetAllRoles();
            var model = new List<CheckElementModel>();
            foreach (var role in roles)
            {
                var userRolesViewModel = new CheckElementModel
                {
                    Id = role.Id,
                    Name = role.Name
                };
                if (await _userService.IsUserHasveRole(id, role.Name))
                {
                    userRolesViewModel.Selected = true;
                }
                else
                {
                    userRolesViewModel.Selected = false;
                }
                model.Add(userRolesViewModel);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageRoles(List<CheckElementModel> model, string id)
        {
            var result = await _userService.AddRolesToUser(id, model.Where(r=>r.Selected).Select(r => r.Name));
            return RedirectToAction("UserRoles");
        }
    }
}
