namespace CyberGooseReview.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<SubCategoryModel> subCategories { get; set; }
    }
}
