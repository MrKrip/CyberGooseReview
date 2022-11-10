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
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Category, CategoryDTO>());
            var mapper = new Mapper(config);
            DataBase.Categories.Create(mapper.Map<Category>(category));
            DataBase.save();
        }

        public void CreateSubCategory(SubCategoryDTO subCategory)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<SubCategory, SubCategoryDTO>());
            var mapper = new Mapper(config);
            DataBase.SubCategories.Create(mapper.Map<SubCategory>(subCategory));
            DataBase.save();
        }

        public void DeleteCategory(int id)
        {
            DataBase.Categories.Delete(id);
            DataBase.save();
        }

        public void DeleteSubCategory(int id)
        {
            DataBase.SubCategories.Delete(id);
            DataBase.save();
        }

        public IEnumerable<ProductDTO> GetAllProducts(int category)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Product, ProductDTO>());
            var mapper = new Mapper(config);
            return DataBase.Products.Find(c => c.CategoryId == category).Select(p => mapper.Map<ProductDTO>(p));
        }

        public IEnumerable<SubCategoryDTO> GetAllSubCatForCat(int categoryId)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<SubCategory, SubCategoryDTO>());
            var mapper = new Mapper(config);
            return DataBase.SubCategories.Find(sc => sc.CategoryId == categoryId).Select(sc => mapper.Map<SubCategoryDTO>(sc));
        }

        public IEnumerable<CategoryDTO> GetCategories()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Category, CategoryDTO>());
            var mapper = new Mapper(config);
            return DataBase.Categories.GetAll().Select(c => mapper.Map<CategoryDTO>(c));
        }

        public ProductDTO GetProduct(int id)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Product, ProductDTO>());
            var mapper = new Mapper(config);
            return mapper.Map<ProductDTO>(DataBase.Products.Get(id));
        }

        public void UpdateCategory(CategoryDTO category)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Category, CategoryDTO>());
            var mapper = new Mapper(config);
            DataBase.Categories.Update(mapper.Map<Category>(category));
            DataBase.save();
        }

        public void UpdateSubCategory(SubCategoryDTO subCategory)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<SubCategory, SubCategoryDTO>());
            var mapper = new Mapper(config);
            DataBase.SubCategories.Update(mapper.Map<SubCategory>(subCategory));
            DataBase.save();
        }
    }
}
