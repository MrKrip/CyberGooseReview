using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using DAL.Entity;
using DAL.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BLL.Services
{
    public class ProductService : IProductService
    {
        private IUnitOfWork DataBase;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ProductService(IUnitOfWork db, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            DataBase = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task AddRolesToCat(int categoryId, List<RoleDTO> roles)
        {
            var Cat = DataBase.Categories.Get(categoryId);
            foreach (RoleDTO role in roles)
            {
                var CatRole = DataBase.CategoryRoles.Find(cr => cr.RoleID == role.Id && cr.CategoryId == categoryId);
                if (CatRole.Any())
                {
                    if (!role.Selected)
                    {
                        DataBase.CategoryRoles.Delete(CatRole.FirstOrDefault().Id);
                        DataBase.save();
                    };
                }
                else
                {
                    if (role.Selected)
                    {
                        var Role = await _roleManager.FindByIdAsync(role.Id);
                        CategoryRoles temp = new CategoryRoles()
                        {
                            CategoryId = categoryId,
                            Category = Cat,
                            RoleID = Role.Id
                        };
                        DataBase.CategoryRoles.Create(temp);
                        DataBase.save();
                    }
                }
            }
        }

        public void CreateCategory(CategoryDTO category)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<CategoryDTO, Category>());
            var mapper = new Mapper(config);
            DataBase.Categories.Create(mapper.Map<Category>(category));
            DataBase.save();
            var CurrCat = DataBase.Categories.Find(c => c.Name == category.Name).FirstOrDefault();
            foreach (var sc in category.subCategories)
            {
                CategoriesSubCategories temp = new CategoriesSubCategories()
                {
                    CategoryId = CurrCat.Id,
                    Category = CurrCat,
                    SubCategoryId = sc.Id,
                    SubCategory = DataBase.SubCategories.Get(sc.Id)
                };
                DataBase.CategoriesSubCategories.Create(temp);
                DataBase.save();
            }
        }

        public void CreateProduct(ProductDTO product)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CategoryDTO, Category>();
                cfg.CreateMap<SubCategoryDTO, SubCategory>();
            });
            var mapper = new Mapper(config);
            DataBase.Products.Create(new Product()
            {
                Category = DataBase.Categories.Get(product.Id),
                CategoryId = product.CategoryId,
                Country = product.Country,
                Year = product.Year,
                Name = product.Name,
                ProductPicture = product.ProductPicture,
                Description = product.Description,
                YouTubeLink = product.YouTubeLink,
                UserRating = 0,
                CommonRating = 0,
                CriticRating = 0
            });
            DataBase.save();
            foreach (var sc in product.SubCategories)
            {
                var prod = DataBase.Products.Find(p => p.Name == product.Name).FirstOrDefault();
                ProductSubCategories temp = new ProductSubCategories()
                {
                    Product = prod,
                    ProductId = prod.Id,
                    SubCategory = mapper.Map<SubCategory>(sc),
                    SubCategoryId = sc.Id
                };
                DataBase.ProductSubCategories.Create(temp);
                DataBase.save();
            }
        }

        public bool CreateSubCategory(SubCategoryDTO subCategory)
        {
            if (DataBase.SubCategories.Find(sc => sc.Name.ToLower() == subCategory.Name.ToLower()).Any())
            {
                return false;
            }
            var config = new MapperConfiguration(cfg => cfg.CreateMap<SubCategoryDTO, SubCategory>());
            var mapper = new Mapper(config);
            DataBase.SubCategories.Create(mapper.Map<SubCategory>(subCategory));
            DataBase.save();
            return true;
        }

        public void DeleteCategory(int id)
        {
            DataBase.Categories.Delete(id);
            DataBase.save();
        }

        public void DeleteProduct(int id)
        {
            DataBase.Products.Delete(id);
            DataBase.save();
        }

        public void DeleteSubCategory(int id)
        {
            DataBase.SubCategories.Delete(id);
            DataBase.save();
        }

        public IEnumerable<ProductDTO> FindProducts(Func<Product, bool> predicate)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Category, CategoryDTO>());
            var mapper = new Mapper(config);
            return DataBase.Products.Find(predicate).ToList().Select(p => new ProductDTO()
            {
                Id = p.Id,
                Name = p.Name,
                Category = mapper.Map<CategoryDTO>(p.Category),
                CategoryId = p.CategoryId,
                CommonRating = p.CommonRating,
                Country = p.Country,
                CriticRating = p.CriticRating,
                UserRating = p.UserRating,
                Description = p.Description,
                ProductPicture = p.ProductPicture,
                Year = p.Year,
                YouTubeLink = p.YouTubeLink,
                SubCategories = DataBase.ProductSubCategories.Find(psc => psc.ProductId == p.Id).Select(psc => new SubCategoryDTO() { Id = psc.Id, Name = psc.SubCategory.Name })
            });
        }

        public IEnumerable<SubCategoryDTO> FindSubCategories(Func<SubCategory, bool> predicate)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<SubCategory, SubCategoryDTO>());
            var mapper = new Mapper(config);
            return DataBase.SubCategories.Find(predicate).Select(sc => mapper.Map<SubCategoryDTO>(sc));
        }

        public IEnumerable<ProductDTO> GetAllProducts()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SubCategory, SubCategoryDTO>();
                cfg.CreateMap<Category, CategoryDTO>();
            });
            var mapper = new Mapper(config);


            return DataBase.Products.GetAll().ToList().Select(p => new ProductDTO()
            {
                Id = p.Id,
                Name = p.Name,
                YouTubeLink = p.YouTubeLink,
                Description = p.Description,
                CategoryId = p.CategoryId,
                Category = mapper.Map<CategoryDTO>(p.Category),
                CommonRating = p.CommonRating,
                Country = p.Country,
                CriticRating = p.CriticRating,
                ProductPicture = p.ProductPicture,
                UserRating = p.UserRating,
                Year = p.Year,
                SubCategories = DataBase.ProductSubCategories.Find(psc => psc.ProductId == p.Id).Select(psc => new SubCategoryDTO() { Id = psc.Id, Name = DataBase.SubCategories.Get(psc.SubCategoryId).Name }).ToList()
            });
        }

        public IEnumerable<ProductDTO> GetAllProductsByCategory(int category)
        {
            var config = new MapperConfiguration(cfg => { cfg.CreateMap<Product, ProductDTO>(); cfg.CreateMap<SubCategory, SubCategoryDTO>(); });
            var mapper = new Mapper(config);
            return DataBase.Products.Find(c => c.CategoryId == category).Select(p => mapper.Map<ProductDTO>(p));
        }

        public IEnumerable<SubCategoryDTO> GetAllSubCategories()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<SubCategory, SubCategoryDTO>());
            var mapper = new Mapper(config);
            var a = DataBase.SubCategories.GetAll().ToList();
            return DataBase.SubCategories.GetAll().Select(sc => mapper.Map<SubCategoryDTO>(sc)).ToList();
        }

        public IEnumerable<SubCategoryDTO> GetAllSubCatForCat(int categoryId)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<SubCategory, SubCategoryDTO>());
            var mapper = new Mapper(config);
            return DataBase.SubCategories.GetAll().ToList().Where(sc => DataBase.CategoriesSubCategories.Find(c => c.CategoryId == categoryId).Any(c => c.SubCategoryId == sc.Id)).Select(sc => mapper.Map<SubCategoryDTO>(sc));
        }

        public IEnumerable<CategoryDTO> GetCategories()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<SubCategory, SubCategoryDTO>());
            var mapper = new Mapper(config);
            return DataBase.Categories.GetAll().ToList().Select(c => new CategoryDTO()
            {
                Id = c.Id,
                Name = c.Name,
                subCategories = DataBase.CategoriesSubCategories.Find(csc => csc.CategoryId == c.Id).Select(csc => mapper.Map<SubCategoryDTO>(DataBase.SubCategories.Get(csc.SubCategoryId)))
            });
        }

        public CategoryDTO GetCategory(int id)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Category, CategoryDTO>());
            var mapper = new Mapper(config);
            return mapper.Map<CategoryDTO>(DataBase.Categories.Get(id));
        }

        public ProductDTO GetProduct(int id)
        {
            var config = new MapperConfiguration(cfg => { cfg.CreateMap<Product, ProductDTO>(); cfg.CreateMap<SubCategory, SubCategoryDTO>(); });
            var mapper = new Mapper(config);
            return mapper.Map<ProductDTO>(DataBase.Products.Get(id));
        }

        public IEnumerable<IdentityRole> GetRolesForCat(int CatId)
        {
            return DataBase.CategoryRoles.Find(cr => cr.CategoryId == CatId).Select(cr => cr.Role);
        }

        public bool HasSubCategory(int categoryId, int subCategoryId)
        {
            return DataBase.CategoriesSubCategories.Find(c => c.CategoryId == categoryId && c.SubCategoryId == subCategoryId).Any();
        }

        public bool IsCategoryHasRole(int categoryId, string roleId)
        {
            return DataBase.CategoryRoles.Find(cr => cr.RoleID == roleId && cr.CategoryId == categoryId).Any();
        }

        public void UpdateCategory(CategoryDTO category)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<CategoryDTO, Category>());
            var mapper = new Mapper(config);
            DataBase.Categories.Update(mapper.Map<Category>(category));
            DataBase.save();
            var CatSubCat = DataBase.CategoriesSubCategories.Find(csc => csc.CategoryId == category.Id).Where(csc => !category.subCategories.Where(sc => sc.Id == csc.SubCategoryId).Any());
            foreach (var cat in CatSubCat)
            {
                DataBase.CategoriesSubCategories.Delete(cat.Id);
            }
            var CurrCat = DataBase.Categories.Get(category.Id);
            foreach (var sc in category.subCategories)
            {
                if (!DataBase.CategoriesSubCategories.Find(csc => csc.CategoryId == category.Id && csc.SubCategoryId == sc.Id).Any())
                {
                    CategoriesSubCategories temp = new CategoriesSubCategories()
                    {
                        CategoryId = CurrCat.Id,
                        Category = CurrCat,
                        SubCategoryId = sc.Id,
                        SubCategory = DataBase.SubCategories.Get(sc.Id)
                    };
                    DataBase.CategoriesSubCategories.Create(temp);
                    DataBase.save();
                }
            }
        }

        public void UpdateProduct(ProductDTO product)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Category, CategoryDTO>());
            var mapper = new Mapper(config);
            var RealProduct = DataBase.Products.Get(product.Id);
            if (RealProduct.CategoryId != product.CategoryId)
            {
                var ProdSubCat = DataBase.ProductSubCategories.Find(psc => psc.ProductId == product.Id);
                foreach (var prodsubcat in ProdSubCat)
                {
                    DataBase.ProductSubCategories.Delete(prodsubcat.Id);
                }
            }
            if (product.ProductPicture != null && product.ProductPicture.Length > 0)
            {
                RealProduct.ProductPicture = product.ProductPicture;
            }
            RealProduct.CategoryId = product.CategoryId;
            RealProduct.Name = product.Name;
            RealProduct.Description = product.Description;
            RealProduct.Category = DataBase.Categories.Get(product.CategoryId);
            RealProduct.YouTubeLink = product.YouTubeLink;
            RealProduct.Year = product.Year;
            RealProduct.Country = product.Country;
            DataBase.Products.Update(RealProduct);
            DataBase.save();
        }

        public void UpdateSubCategory(SubCategoryDTO subCategory)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<SubCategoryDTO, SubCategory>());
            var mapper = new Mapper(config);
            DataBase.SubCategories.Update(mapper.Map<SubCategory>(subCategory));
            DataBase.save();
        }

        IEnumerable<CategoryDTO> IProductService.FindCategories(Func<Category, bool> predicate)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<SubCategory, SubCategoryDTO>());
            var mapper = new Mapper(config);
            return DataBase.Categories.Find(predicate).Select(c => new CategoryDTO()
            {
                Id = c.Id,
                Name = c.Name,
                subCategories = DataBase.CategoriesSubCategories.Find(csc => csc.CategoryId == c.Id).Select(csc => mapper.Map<SubCategoryDTO>(DataBase.SubCategories.Get(csc.SubCategoryId)))
            });
        }

        public bool IsProductHasSubCat(int productId, int subCatId)
        {
            return DataBase.ProductSubCategories.Find(psc => psc.ProductId == productId && psc.SubCategoryId == subCatId).Any();
        }

        public void AddSubCategoriesToProduct(int productId, List<SubCatCheckDTO> SubCategories)
        {
            var Product = DataBase.Products.Get(productId);
            foreach (var SubCategory in SubCategories)
            {
                var SubCat = DataBase.ProductSubCategories.Find(psc => psc.ProductId == productId && psc.SubCategoryId == SubCategory.Id);
                if (SubCat.Any())
                {
                    if (!SubCategory.Selected)
                    {
                        DataBase.ProductSubCategories.Delete(SubCat.FirstOrDefault().Id);
                        DataBase.save();
                    };
                }
                else
                {
                    if (SubCategory.Selected)
                    {
                        var SubCatTemp = DataBase.SubCategories.Get(SubCategory.Id);
                        ProductSubCategories temp = new ProductSubCategories()
                        {
                            Product = Product,
                            ProductId = Product.Id,
                            SubCategory = SubCatTemp,
                            SubCategoryId = SubCatTemp.Id
                        };
                        DataBase.ProductSubCategories.Create(temp);
                        DataBase.save();
                    }
                }
            }
        }
    }
}
