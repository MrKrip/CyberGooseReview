using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using CyberGooseReview.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
                ItemsPageModel<SubCategoryModel> model = new ItemsPageModel<SubCategoryModel>(subCat.Reverse(), pageModel);
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
                    ItemsPageModel<SubCategoryModel> model = new ItemsPageModel<SubCategoryModel>(subCat.Reverse(), pageModel);
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Subcategory \"{SubCatName}\" does not exist");
                    subCat = _productService.GetAllSubCategories().Select(sc => mapper.Map<SubCategoryModel>(sc));
                    var count = subCat.Count();
                    subCat = subCat.Skip((page - 1) * pageSize).Take(pageSize);
                    PageModel pageModel = new PageModel(count, page, pageSize);
                    ItemsPageModel<SubCategoryModel> model = new ItemsPageModel<SubCategoryModel>(subCat.Reverse(), pageModel);
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
                var Categories = _productService.GetCategories().Select(sc => new CategoryModel()
                {
                    subCategories = sc.subCategories.Select(SC => mapper.Map<SubCategoryModel>(SC)),
                    Id = sc.Id,
                    Name = sc.Name,
                    Roles = _productService.GetRolesForCat(sc.Id).Select(rc => rc.Name)
                });
                var count = Categories.Count();
                Categories = Categories.Skip((page - 1) * pageSize).Take(pageSize);
                PageModel pageModel = new PageModel(count, page, pageSize);
                ItemsPageModel<CategoryModel> model = new ItemsPageModel<CategoryModel>(Categories.Reverse(), pageModel);
                return View(model);
            }
            else
            {
                var Categories = _productService.FindCategories(c => c.Name.ToLower().Contains(CatName.ToLower()))
                    .Select(sc => new CategoryModel()
                    {
                        subCategories = sc.subCategories.Select(SC => mapper.Map<SubCategoryModel>(SC)),
                        Id = sc.Id,
                        Name = sc.Name,
                        Roles = _productService.GetRolesForCat(sc.Id).Select(rc => rc.Name)
                    });
                if (Categories.Any())
                {
                    ViewBag.category = CatName;
                    var count = Categories.Count();
                    Categories = Categories.Skip((page - 1) * pageSize).Take(pageSize);
                    PageModel pageModel = new PageModel(count, page, pageSize);
                    ItemsPageModel<CategoryModel> model = new ItemsPageModel<CategoryModel>(Categories.Reverse(), pageModel);
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Subcategory \"{CatName}\" does not exist");
                    Categories = _productService.GetCategories()
                        .Select(sc => new CategoryModel()
                        {
                            subCategories = sc.subCategories.Select(SC => mapper.Map<SubCategoryModel>(SC)),
                            Id = sc.Id,
                            Name = sc.Name,
                            Roles = _productService.GetRolesForCat(sc.Id).Select(rc => rc.Name)
                        });
                    var count = Categories.Count();
                    Categories = Categories.Skip((page - 1) * pageSize).Take(pageSize);
                    PageModel pageModel = new PageModel(count, page, pageSize);
                    ItemsPageModel<CategoryModel> model = new ItemsPageModel<CategoryModel>(Categories.Reverse(), pageModel);
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

        [Authorize(Roles = "Moderator,Admin")]
        public IActionResult Products(string? ProductName, int page = 1)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SubCategoryDTO, SubCategoryModel>();
                cfg.CreateMap<CategoryDTO, CategoryModel>();
            });
            var mapper = new Mapper(config);
            if (ProductName == null)
            {
                var Products = _productService.GetAllProducts().Select(p => new ProductModel()
                {
                    Name = p.Name,
                    Id = p.Id,
                    CategoryId = p.CategoryId,
                    CriticRating = p.CriticRating,
                    UserRating = p.UserRating,
                    SubCategories = p.SubCategories.Select(sc => new SubCategoryModel() { Id = sc.Id, Name = sc.Name }).ToList(),
                    Category = mapper.Map<CategoryModel>(p.Category)
                });
                var count = Products.ToList().Count();
                Products = Products.Skip((page - 1) * pageSize).Take(pageSize);
                PageModel pageModel = new PageModel(count, page, pageSize);
                ItemsPageModel<ProductModel> model = new ItemsPageModel<ProductModel>(Products.Reverse(), pageModel);
                return View(model);
            }
            else
            {
                var Products = _productService.FindProducts(c => c.Name.ToLower().Contains(ProductName.ToLower())).Select(p => new ProductModel()
                {
                    Name = p.Name,
                    Id = p.Id,
                    CategoryId = p.CategoryId,
                    CriticRating = p.CriticRating,
                    UserRating = p.UserRating,
                    SubCategories = p.SubCategories.Select(sc => mapper.Map<SubCategoryModel>(sc)).ToList(),
                    Category = mapper.Map<CategoryModel>(p.Category)
                }); ;
                if (Products.Any())
                {
                    ViewBag.product = ProductName;
                    var count = Products.Count();
                    Products = Products.Skip((page - 1) * pageSize).Take(pageSize);
                    PageModel pageModel = new PageModel(count, page, pageSize);
                    ItemsPageModel<ProductModel> model = new ItemsPageModel<ProductModel>(Products.Reverse(), pageModel);
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Product \"{ProductName}\" does not exist");
                    Products = _productService.GetAllProducts().Select(p => new ProductModel()
                    {
                        Name = p.Name,
                        Id = p.Id,
                        CategoryId = p.CategoryId,
                        CriticRating = p.CriticRating,
                        UserRating = p.UserRating,
                        SubCategories = p.SubCategories.Select(sc => mapper.Map<SubCategoryModel>(sc)).ToList(),
                        Category = mapper.Map<CategoryModel>(p.Category)
                    }); ;
                    var count = Products.Count();
                    Products = Products.Skip((page - 1) * pageSize).Take(pageSize);
                    PageModel pageModel = new PageModel(count, page, pageSize);
                    ItemsPageModel<ProductModel> model = new ItemsPageModel<ProductModel>(Products.Reverse(), pageModel);
                    return View(model);
                }
            }
        }

        [Authorize(Roles = "Moderator,Admin")]
        [HttpGet]
        public IActionResult NewProduct()
        {
            return View();
        }

        [Authorize(Roles = "Moderator,Admin")]
        [HttpPost]
        public async Task<IActionResult> NewProductAsync(ProductManageModel model)
        {
            if (Request.Form.Files.Count > 0)
            {
                IFormFile file = Request.Form.Files.FirstOrDefault();
                using (var dataStream = new MemoryStream())
                {
                    await file.CopyToAsync(dataStream);
                    model.ProductPicture = dataStream.ToArray();
                }
            }
            if (ModelState.IsValid)
            {
                _productService.CreateProduct(new ProductDTO()
                {
                    CategoryId = model.CategoryId,
                    Name = model.Name,
                    Description = model.Description,
                    Category = _productService.GetCategory(model.CategoryId),
                    YouTubeLink = model.YouTubeLink,
                    SubCategories = new List<SubCategoryDTO>(),
                    ProductPicture = model.ProductPicture,
                    Year = model.Year,
                    Country = model.Country
                });
                return RedirectToAction("Products");
            }
            return View(model);
        }

        [Authorize(Roles = "Moderator,Admin")]
        [HttpGet]
        public IActionResult RolesCategory(int id)
        {
            ViewBag.Id = id;
            ViewBag.Category = _productService.GetCategory(id).Name;
            var roles = _userService.GetAllRoles();
            var model = new List<CheckElementModel>();
            foreach (var role in roles)
            {
                var categoryRolesViewModel = new CheckElementModel
                {
                    Id = role.Id,
                    Name = role.Name
                };
                if (_productService.IsCategoryHasRole(id, role.Id))
                {
                    categoryRolesViewModel.Selected = true;
                }
                else
                {
                    categoryRolesViewModel.Selected = false;
                }
                model.Add(categoryRolesViewModel);
            }
            return View(model);
        }

        [Authorize(Roles = "Moderator,Admin")]
        [HttpPost]
        public async Task<IActionResult> RolesCategory(List<CheckElementModel> model, int id)
        {
            await _productService.AddRolesToCat(id, model.Select(m => new RoleDTO() { Id = m.Id, Name = m.Name, Selected = m.Selected }).ToList());
            return RedirectToAction("Categories");
        }

        [Authorize(Roles = "Moderator,Admin")]
        [HttpGet]
        public IActionResult ProductSubCat(int id, int CatId)
        {
            ViewBag.Id = id;
            ViewBag.Product = _productService.GetProduct(id).Name;
            var SubCat = _productService.GetAllSubCatForCat(CatId);
            var model = new List<CheckElementModel>();
            foreach (var subCategory in SubCat)
            {
                var ProductSubCatModel = new CheckElementModel
                {
                    Id = subCategory.Id.ToString(),
                    Name = subCategory.Name
                };
                if (_productService.IsProductHasSubCat(id, subCategory.Id))
                {
                    ProductSubCatModel.Selected = true;
                }
                else
                {
                    ProductSubCatModel.Selected = false;
                }
                model.Add(ProductSubCatModel);
            }
            return View(model);
        }


        [Authorize(Roles = "Moderator,Admin")]
        [HttpPost]
        public IActionResult ProductSubCat(List<CheckElementModel> model, int id)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<CheckElementModel, SubCatCheckDTO>());
            var mapper = new Mapper(config);
            _productService.AddSubCategoriesToProduct(id, model.Select(m => mapper.Map<SubCatCheckDTO>(m)).ToList());
            return RedirectToAction("Products");
        }

        [Authorize(Roles = "Moderator,Admin")]
        public IActionResult DeleteProduct(int id)
        {
            _productService.DeleteProduct(id);
            return RedirectToAction("Products");
        }

        [Authorize(Roles = "Moderator,Admin")]
        [HttpGet]
        public IActionResult ManageProduct(int id)
        {
            ViewBag.Id = id;
            var config = new MapperConfiguration(cfg => cfg.CreateMap<ProductDTO, ProductManageModel>());
            var mapper = new Mapper(config);
            return View(mapper.Map<ProductManageModel>(_productService.GetProduct(id)));
        }

        [Authorize(Roles = "Moderator,Admin")]
        [HttpPost]
        public async Task<IActionResult> ManageProduct(ProductManageModel model, int Id)
        {
            if (Request.Form.Files.Count > 0)
            {
                IFormFile file = Request.Form.Files.FirstOrDefault();
                using (var dataStream = new MemoryStream())
                {
                    await file.CopyToAsync(dataStream);
                    model.ProductPicture = dataStream.ToArray();
                }
            }
            if (ModelState.IsValid)
            {
                _productService.UpdateProduct(new ProductDTO()
                {
                    Id = Id,
                    CategoryId = model.CategoryId,
                    Name = model.Name,
                    Description = model.Description,
                    Category = _productService.GetCategory(model.CategoryId),
                    YouTubeLink = model.YouTubeLink,
                    ProductPicture = model.ProductPicture,
                    Year = model.Year,
                    Country = model.Country
                });
                return RedirectToAction("Products");
            }
            return View(model);
        }

        public IActionResult Search(string ProductName)
        {
            var Product = _productService.FindProducts(p => p.Name.ToLower().Contains(ProductName.ToLower())).Select(p => new ProductModel()
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
            return View(Product);
        }

        public IActionResult Product(int Id)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<CategoryDTO, CategoryModel>());
            var mapper = new Mapper(config);
            var p = _productService.GetProduct(Id);
            var ProductModel = new ProductModel()
            {
                Name = p.Name,
                Description = p.Description,
                ProductPicture = p.ProductPicture,
                Id = p.Id,
                UserRating = p.UserRating,
                Country = p.Country,
                CommonRating = p.CommonRating,
                CriticRating = p.CriticRating,
                Year = p.Year,
                CategoryId = p.CategoryId,
                Category = mapper.Map<CategoryModel>(_productService.GetCategory(p.CategoryId)),
                SubCategories = _productService.GetAllSubCatForCat(p.CategoryId).Select(sc => new SubCategoryModel()
                {
                    Id = sc.Id,
                    Name = sc.Name
                }).ToList(),
                YouTubeLink = p.YouTubeLink
            };
            ItemWithReviewModel<ProductModel> model = new ItemWithReviewModel<ProductModel>()
            {
                Item = ProductModel,
                Reviews = _reviewService.GetAllReviewsToProduct(ProductModel.Id).Reverse().Select(r => new ReviewModel()
                {
                    CreationDate = r.CreationDate,
                    Details = r.ReviewDetails,
                    DisLikes = r.DisLikes,
                    Likes = r.Likes,
                    userData = new UserDataModel(r.UserId, _userService, true) { Roles = _userService.CriticRoles(r.UserId, ProductModel.CategoryId).Result },
                    ProductId = ProductModel.Id,
                    ProductName = ProductModel.Name,
                    Rating = r.Rating,
                }),
                UserReview = _reviewService.FindUserReviews(r => r.UserId == _userService.GetCurrentUser(User).Id && r.ProductId == ProductModel.Id).Select(r => new UserReviewModel()
                {
                    ProductId = r.ProductId,
                    Rating = r.Rating,
                    ReviewDetails = r.ReviewDetails,
                    UserId = r.UserId
                }).FirstOrDefault()
            };
            return View(model);
        }

        public IActionResult Reviews()
        {
            return View();
        }

        public IActionResult Category()
        {
            var Products = _productService.GetAllProducts().Where(p => p.CategoryId == 1).Select(p => new ProductModel()
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
            return View(Products);
        }

        public IActionResult Random()
        {
            return View();
        }
    }
}
