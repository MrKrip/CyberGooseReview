﻿using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using CyberGooseReview.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CyberGooseReview.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IProductService _productService;
        private IReviewService _reviewService;
        private IUserService _userService;
        private int pageSize = 10;

        public ProductController(ILogger<HomeController> logger, IReviewService reviewService, IUserService userService, IProductService productService)
        {
            _logger = logger;
            _reviewService = reviewService;
            _userService = userService;
            _productService = productService;
        }

        [Authorize(Roles = "Moderator,Admin")]
        public IActionResult SubCategories(string? SubCatName, int page = 1)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<SubCategoryDTO, SubCategoryModel>());
            var mapper = new Mapper(config);
            if (SubCatName == null)
            {
                var subCat = _productService.GetAllSubCategories().Select(sc => mapper.Map<SubCategoryModel>(sc));
                var count = subCat.Count();
                subCat = subCat.Skip((page - 1) * pageSize).Take(pageSize);
                PageModel pageModel = new PageModel(count, page, pageSize);
                ItemsPageModel<SubCategoryModel> model = new ItemsPageModel<SubCategoryModel>(subCat, pageModel);
                return View(model);
            }
            else
            {
                var subCat = _productService.FindSubCategories(sc => sc.Name.ToLower().Contains(SubCatName.ToLower())).Select(sc => mapper.Map<SubCategoryModel>(sc));
                if (subCat.Any())
                {
                    ViewBag.subCat = SubCatName;
                    var count = subCat.Count();
                    subCat = subCat.Skip((page - 1) * pageSize).Take(pageSize);
                    PageModel pageModel = new PageModel(count, page, pageSize);
                    ItemsPageModel<SubCategoryModel> model = new ItemsPageModel<SubCategoryModel>(subCat, pageModel);
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Subcategory \"{SubCatName}\" does not exist");
                    subCat = _productService.GetAllSubCategories().Select(sc => mapper.Map<SubCategoryModel>(sc));
                    var count = subCat.Count();
                    subCat = subCat.Skip((page - 1) * pageSize).Take(pageSize);
                    PageModel pageModel = new PageModel(count, page, pageSize);
                    ItemsPageModel<SubCategoryModel> model = new ItemsPageModel<SubCategoryModel>(subCat, pageModel);
                    return View(model);
                }
            }
        }

        [Authorize(Roles = "Moderator,Admin")]
        public IActionResult NewSubCat(string subCatName)
        {
            if (string.IsNullOrEmpty(subCatName))
            {
                ModelState.AddModelError(string.Empty, "The \"New Subcategory\" field must not be empty.");
                return RedirectToAction("SubCategories");
            }
            var result = _productService.CreateSubCategory(new SubCategoryDTO() { Name = subCatName });
            if (!result)
            {
                ModelState.AddModelError(string.Empty, $"Subcategory \"{subCatName}\" already exists");
            }
            return RedirectToAction("SubCategories");
        }

        [Authorize(Roles = "Moderator,Admin")]
        public IActionResult DeleteSubCat(int id)
        {
            _productService.DeleteSubCategory(id);
            return RedirectToAction("SubCategories");
        }

        [Authorize(Roles = "Moderator,Admin")]
        public IActionResult Categories(string? CatName, int page = 1)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<SubCategoryDTO, SubCategoryModel>());
            var mapper = new Mapper(config);
            if (CatName == null)
            {
                var Categories = _productService.GetCategories().Select(sc => new CategoryModel() { subCategories = sc.subCategories.Select(SC => mapper.Map<SubCategoryModel>(SC)), Id = sc.Id, Name = sc.Name });
                var count = Categories.Count();
                Categories = Categories.Skip((page - 1) * pageSize).Take(pageSize);
                PageModel pageModel = new PageModel(count, page, pageSize);
                ItemsPageModel<CategoryModel> model = new ItemsPageModel<CategoryModel>(Categories, pageModel);
                return View(model);
            }
            else
            {
                var Categories = _productService.FindCategories(c => c.Name.ToLower().Contains(CatName.ToLower()))
                    .Select(sc => new CategoryModel() { subCategories = sc.subCategories.Select(SC => mapper.Map<SubCategoryModel>(SC)), Id = sc.Id, Name = sc.Name });
                if (Categories.Any())
                {
                    ViewBag.category = CatName;
                    var count = Categories.Count();
                    Categories = Categories.Skip((page - 1) * pageSize).Take(pageSize);
                    PageModel pageModel = new PageModel(count, page, pageSize);
                    ItemsPageModel<CategoryModel> model = new ItemsPageModel<CategoryModel>(Categories, pageModel);
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Subcategory \"{CatName}\" does not exist");
                    Categories = _productService.GetCategories()
                        .Select(sc => new CategoryModel() { subCategories = sc.subCategories.Select(SC => mapper.Map<SubCategoryModel>(SC)), Id = sc.Id, Name = sc.Name });
                    var count = Categories.Count();
                    Categories = Categories.Skip((page - 1) * pageSize).Take(pageSize);
                    PageModel pageModel = new PageModel(count, page, pageSize);
                    ItemsPageModel<CategoryModel> model = new ItemsPageModel<CategoryModel>(Categories, pageModel);
                    return View(model);
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Moderator,Admin")]
        public IActionResult NewCategory()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<SubCategoryDTO, SubCategoryModel>());
            var mapper = new Mapper(config);
            var subCat = _productService.GetAllSubCategories().Select(sc => mapper.Map<SubCategoryModel>(sc));
            var model = new CategoryManageModel() { Id = null, Name = string.Empty, subCategories = subCat.Select(sc => new SubCatCheckModel() { Id = sc.Id, Name = sc.Name, Selected = false }).ToList() };
            return View(model);
        }

        [Authorize(Roles = "Moderator,Admin")]
        [HttpPost]
        public IActionResult NewCategory(CategoryManageModel model)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<SubCatCheckModel, SubCategoryDTO>());
            var mapper = new Mapper(config);
            _productService.CreateCategory(new CategoryDTO() { Name = model.Name, subCategories = model.subCategories.Where(sc => sc.Selected).Select(sc => mapper.Map<SubCategoryDTO>(sc)) });
            return RedirectToAction("Categories");
        }

        [Authorize(Roles = "Moderator,Admin")]
        public IActionResult DeleteCategory(int id)
        {
            _productService.DeleteCategory(id);
            return RedirectToAction("Categories");
        }

        [HttpGet]
        [Authorize(Roles = "Moderator,Admin")]
        public IActionResult ManageCategory(int id)
        {
            ViewBag.Id = id;
            var config = new MapperConfiguration(cfg => cfg.CreateMap<SubCategoryDTO, SubCategoryModel>());
            var mapper = new Mapper(config);
            var subCat = _productService.GetAllSubCategories().Select(sc => mapper.Map<SubCategoryModel>(sc));
            var Category = _productService.GetCategory(id);
            ViewBag.CategoryName = Category.Name;
            var model = new CategoryManageModel()
            {
                Id = Category.Id,
                Name = Category.Name,
                subCategories = subCat.Select(sc => new SubCatCheckModel() { Id = sc.Id, Name = sc.Name, Selected = _productService.HasSubCategory(Category.Id, sc.Id) }).ToList()
            };
            return View(model);
        }

        [Authorize(Roles = "Moderator,Admin")]
        [HttpPost]
        public IActionResult ManageCategory(CategoryManageModel model, int Id)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<SubCatCheckModel, SubCategoryDTO>());
            var mapper = new Mapper(config);
            _productService.UpdateCategory(new CategoryDTO() { Id = Id, Name = model.Name, subCategories = model.subCategories.Where(sc => sc.Selected).Select(sc => mapper.Map<SubCategoryDTO>(sc)) });
            return RedirectToAction("Categories");
        }

        public IActionResult Search(string ProductName)
        {
            return View();
        }
    }
}
