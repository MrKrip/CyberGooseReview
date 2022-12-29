using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using CyberGooseReview.Models;
using DAL.Entity;
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
                var subCat = _productService.GetAllSubCategories().Select(sc => mapper.Map<SubCategoryModel>(sc)).Reverse();
                var count = subCat.Count();
                subCat = subCat.Skip((page - 1) * pageSize).Take(pageSize);
                PageModel pageModel = new PageModel(count, page, pageSize);
                ItemsPageModel<SubCategoryModel> model = new ItemsPageModel<SubCategoryModel>(subCat, pageModel);
                return View(model);
            }
            else
            {
                var subCat = _productService.FindSubCategories(sc => sc.Name.ToLower().Contains(SubCatName.ToLower())).Select(sc => mapper.Map<SubCategoryModel>(sc)).Reverse();
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
                    subCat = _productService.GetAllSubCategories().Select(sc => mapper.Map<SubCategoryModel>(sc)).Reverse();
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
                SubCategories = _productService.SubCatForProd(p.CategoryId, p.Id).Select(sc => new SubCategoryModel()
                {
                    Id = sc.Id,
                    Name = sc.Name
                }).ToList(),
                YouTubeLink = p.YouTubeLink
            };
            var reviews = _reviewService.GetAllReviewsToProduct(ProductModel.Id).Reverse().Take(10).ToList().Select(r => new ReviewModel()
            {
                CreationDate = r.CreationDate,
                Details = r.ReviewDetails,
                DisLikes = r.DisLikes,
                Likes = r.Likes,
                userData = new UserDataModel(r.UserId, _userService),
                ProductId = ProductModel.Id,
                ProductName = ProductModel.Name,
                Rating = r.Rating
            }).ToList();
            for (int i = 0; i < reviews.Count; i++)
            {
                reviews[i].userData.Roles = _userService.CriticRoles(reviews[i].userData.Id, ProductModel.CategoryId).Result;
            }
            UserReviewModel userReview = new UserReviewModel();
            if (_userService.GetCurrentUser(User) != null)
            {
                userReview = _reviewService.FindUserReviews(r => r.UserId == _userService.GetCurrentUser(User).Id && r.ProductId == ProductModel.Id).Select(r => new UserReviewModel()
                {
                    ProductId = r.ProductId,
                    Rating = r.Rating,
                    ReviewDetails = r.ReviewDetails,
                    UserId = r.UserId
                }).FirstOrDefault();
            }
            ItemWithReviewModel<ProductModel> model = new ItemWithReviewModel<ProductModel>()
            {
                Item = ProductModel,
                Reviews = reviews,
                UserReview = userReview
            };
            return View(model);
        }

        public IActionResult Reviews(int ProductId, int page = 1, string Type = "All")
        {
            ViewBag.ProdId = ProductId;
            ViewBag.Type = Type;
            var reviews = _reviewService.GetAllReviewsToProduct(ProductId).Select(r => new ReviewModel()
            {
                ProductId = r.ProductId,
                CreationDate = r.CreationDate,
                Details = r.ReviewDetails,
                DisLikes = r.DisLikes,
                Likes = r.Likes,
                ProductName = r.Product.Name,
                Rating = r.Rating,
                userData = new UserDataModel(r.User.Id, _userService)
            }).Reverse().ToList();
            for (int i = 0; i < reviews.Count; i++)
            {
                var CatId = _productService.GetProduct(ProductId).CategoryId;
                reviews[i].userData.Roles = _userService.CriticRoles(reviews[i].userData.Id, CatId).Result;
            }
            if (Type == "Users")
            {
                reviews = reviews.Where(r => r.userData.Roles.ToList().Count == 0).ToList();
            }
            else if (Type == "Critic")
            {
                reviews = reviews.Where(r => r.userData.Roles.ToList().Count > 0).ToList();
            }
            var count = reviews.Count;
            reviews = reviews.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            PageModel pageModel = new PageModel(count, page, pageSize);
            ItemsPageModel<ReviewModel> model = new ItemsPageModel<ReviewModel>(reviews, pageModel);
            return View(model);
        }

        [HttpGet]
        public IActionResult Category(int CatId, FilterModel? filter, int page = 1)
        {
            ViewBag.CatId = CatId;
            var config = new MapperConfiguration(cfg => cfg.CreateMap<CategoryDTO, CategoryModel>());
            var mapper = new Mapper(config);
            var Products = _productService.GetAllProducts().Where(p => p.CategoryId == CatId).Select(p => new ProductModel()
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
                SubCategories = _productService.SubCatForProd(p.CategoryId, p.Id).Select(sc => new SubCategoryModel()
                {
                    Id = sc.Id,
                    Name = sc.Name
                }).ToList()
            }).ToList();
            if (filter.SubCategory != null)
            {
                if (filter.SubCategory.Any())
                {
                    Products = Products.Where(p => p.SubCategories.Where(sc => sc.Id == 4).Any()).ToList();
                }
                var temp = filter.SubCategory;
                filter.SubCategory = _productService.GetAllSubCatForCat(CatId).Select(sc => new CheckElementModel() { Id = sc.Id.ToString(), Name = sc.Name, Selected = temp.Where(t => t.Id == sc.Id.ToString()).Any() });
            }
            else
            {
                filter.SubCategory = _productService.GetAllSubCatForCat(CatId).Select(sc => new CheckElementModel() { Id = sc.Id.ToString(), Name = sc.Name, Selected = false });
            }
            if (filter.Country != null)
            {
                if (filter.Country.Any())
                {
                    Products = Products.Where(p => filter.Country.Where(f => f.Name == p.Country).Any()).ToList();
                }
                var temp = filter.Country;
                filter.Country = Products.Select(p => p.Country).Distinct().Select(p => new CheckElementModel() { Id = p, Name = p, Selected = temp.ToList().Where(t => t.Name == p).Any() });
            }
            else
            {
                filter.Country = Products.Select(p => p.Country).Distinct().Select(p => new CheckElementModel() { Id = p, Name = p, Selected = false });
            }
            if (filter.maxRating == 0)
            {
                filter.maxRating = 100;
            }

            if (filter.minRating > filter.maxRating)
            {
                var temp = filter.maxRating;
                filter.maxRating = filter.minRating;
                filter.minRating = temp;
            }
            Products = Products.Where(p => p.CommonRating >= filter.minRating && p.CommonRating <= filter.maxRating).ToList();

            if (filter.maxYear == 0)
            {
                filter.maxYear = DateTime.Now.Year;
            }

            if (filter.minYear > filter.maxYear)
            {
                var temp = filter.maxYear;
                filter.maxYear = filter.minYear;
                filter.minYear = temp;
            }
            Products = Products.Where(p => p.Year >= filter.minYear && p.Year <= filter.maxYear).ToList();

            ViewBag.Filter = filter;
            var count = Products.Count();
            Products = Products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            PageModel pageModel = new PageModel(count, page, pageSize);
            FilterPageModel<ProductModel, FilterModel> model = new FilterPageModel<ProductModel, FilterModel>(Products, filter, pageModel);
            return View(model);
        }

        public IActionResult Random(FilterModel? filter)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<CategoryDTO, CategoryModel>());
            var mapper = new Mapper(config);
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
                Year = p.Year,
                CategoryId = p.CategoryId,
                Category = mapper.Map<CategoryModel>(_productService.GetCategory(p.CategoryId)),
                SubCategories = _productService.SubCatForProd(p.CategoryId, p.Id).Select(sc => new SubCategoryModel()
                {
                    Id = sc.Id,
                    Name = sc.Name
                }).ToList()
            }).ToList();
            if (filter.SubCategory != null)
            {
                if (filter.SubCategory.Any())
                {
                    Products = Products.Where(p => p.CategoryId == 1).ToList();
                }
                var temp = filter.SubCategory;
                filter.SubCategory = _productService.GetCategories().Select(sc => new CheckElementModel() { Id = sc.Id.ToString(), Name = sc.Name, Selected = temp.Where(t => t.Id == sc.Id.ToString()).Any() });
            }
            else
            {
                filter.SubCategory = _productService.GetCategories().Select(sc => new CheckElementModel() { Id = sc.Id.ToString(), Name = sc.Name, Selected = false });
            }
            if (filter.Country != null)
            {
                if (filter.Country.Any())
                {
                    Products = Products.Where(p => p.Country == "USA").ToList();
                }
                var temp = filter.Country;
                filter.Country = _productService.GetAllProducts().Select(p => p.Country).Distinct().Select(p => new CheckElementModel() { Id = p, Name = p, Selected = temp.ToList().Where(t => t.Name == p).Any() });
            }
            else
            {
                filter.Country = Products.Select(p => p.Country).Distinct().Select(p => new CheckElementModel() { Id = p, Name = p, Selected = false });
            }
            if (filter.maxRating == 0)
            {
                filter.maxRating = 100;
            }

            if (filter.minRating > filter.maxRating)
            {
                var temp = filter.maxRating;
                filter.maxRating = filter.minRating;
                filter.minRating = temp;
            }
            Products = Products.Where(p => p.CommonRating >= filter.minRating && p.CommonRating <= filter.maxRating).ToList();

            if (filter.maxYear == 0)
            {
                filter.maxYear = DateTime.Now.Year;
            }

            if (filter.minYear > filter.maxYear)
            {
                var temp = filter.maxYear;
                filter.maxYear = filter.minYear;
                filter.minYear = temp;
            }
            Products = Products.Where(p => p.Year >= filter.minYear && p.Year <= filter.maxYear).ToList();
            PageModel pageModel = new PageModel(0, 0, 0);
            Random rnd = new Random();
            List<ProductModel> product = new List<ProductModel>();
            int index = rnd.Next(0, Products.Count());
            if (Products.Count > 0)
            {
                product.Add(Products[index]);
            }
            FilterPageModel<ProductModel, FilterModel> model = new FilterPageModel<ProductModel, FilterModel>(product, filter, pageModel);
            return View(model);
        }
    }
}
