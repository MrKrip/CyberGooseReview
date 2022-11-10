using BLL.DTO;

namespace BLL.Interfaces
{
    public interface IProductService
    {
        public void CreateCategory(CategoryDTO category);
        public void UpdateCategory(CategoryDTO category);
        public void DeleteCategory(int id);
        public void CreateSubCategory(SubCategoryDTO subCategory);
        public void UpdateSubCategory(SubCategoryDTO subCategory);
        public void DeleteSubCategory(int id);
        public IEnumerable<CategoryDTO> GetCategories();
        public IEnumerable<SubCategoryDTO> GetAllSubCatForCat(int categoryId);
        public IEnumerable<ProductDTO> GetAllProducts(int category);
        public ProductDTO GetProduct(int id);
    }
}
