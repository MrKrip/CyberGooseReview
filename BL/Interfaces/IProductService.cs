using BLL.DTO;
using DAL.Entity;
using Microsoft.AspNetCore.Identity;

namespace BLL.Interfaces
{
    public interface IProductService
    {
        public void CreateCategory(CategoryDTO category);
        public void UpdateCategory(CategoryDTO category);
        public void DeleteCategory(int id);
        public bool CreateSubCategory(SubCategoryDTO subCategory);
        public void UpdateSubCategory(SubCategoryDTO subCategory);
        public void DeleteSubCategory(int id);
        public void CreateProduct(ProductDTO product);
        public void UpdateProduct(ProductDTO product);
        public void DeleteProduct(int id);
        public IEnumerable<CategoryDTO> GetCategories();
        public IEnumerable<SubCategoryDTO> GetAllSubCatForCat(int categoryId);
        public IEnumerable<ProductDTO> GetAllProductsByCategory(int category);
        public IEnumerable<ProductDTO> GetAllProducts();
        public IEnumerable<ProductDTO> FindProducts(Func<Product, bool> predicate);
        public IEnumerable<SubCategoryDTO> GetAllSubCategories();
        public IEnumerable<SubCategoryDTO> FindSubCategories(Func<SubCategory, bool> predicate);
        public IEnumerable<CategoryDTO> FindCategories(Func<Category, bool> predicate);
        public ProductDTO GetProduct(int id);
        public CategoryDTO GetCategory(int id);
        public bool HasSubCategory(int categoryId, int subCategoryId);
        public IEnumerable<IdentityRole> GetRolesForCat(int CatId);
        public bool IsCategoryHasRole(int categoryId, string roleId);
        public Task AddRolesToCat(int categoryId, List<RoleDTO> roles);
        public bool IsProductHasSubCat(int productId, int subCatId);
        public void AddSubCategoriesToProduct(int productId, List<SubCatCheckDTO> categories);
        public void UpdateRating(int categoryId);
        public IEnumerable<SubCategoryDTO> SubCatForProd(int categoryId, int prodId);
    }
}
