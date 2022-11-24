using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using DAL.Entity;
using DAL.Interfaces;

namespace BLL.Services
{
    public class ProductService : IProductService
    {
        private IUnitOfWork DataBase;
        public ProductService(IUnitOfWork db)
        {
            DataBase = db;
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
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Category, CategoryDTO>());
            var mapper = new Mapper(config);
            DataBase.Products.Create(mapper.Map<Product>(product));
            DataBase.save();
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
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Product, ProductDTO>());
            var mapper = new Mapper(config);
            return DataBase.Products.Find(predicate).Select(p => mapper.Map<ProductDTO>(p));
        }

        public IEnumerable<SubCategoryDTO> FindSubCategories(Func<SubCategory, bool> predicate)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<SubCategory, SubCategoryDTO>());
            var mapper = new Mapper(config);
            return DataBase.SubCategories.Find(predicate).Select(sc => mapper.Map<SubCategoryDTO>(sc));
        }

        public IEnumerable<ProductDTO> GetAllProducts(int category)
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
            return DataBase.SubCategories.Find(sc => DataBase.CategoriesSubCategories.Find(c => c.CategoryId == categoryId).Any(c => c.SubCategoryId == sc.Id)).Select(sc => mapper.Map<SubCategoryDTO>(sc));
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

        public bool HasSubCategory(int categoryId, int subCategoryId)
        {
            return DataBase.CategoriesSubCategories.Find(c => c.CategoryId == categoryId && c.SubCategoryId == subCategoryId).Any();
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
            DataBase.Products.Update(mapper.Map<Product>(product));
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
    }
}
