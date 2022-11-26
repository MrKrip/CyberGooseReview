namespace CyberGooseReview.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public CategoryModel Category { get; set; }

        public List<SubCategoryModel> SubCategories { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? YouTubeLink { get; set; }
        public int UserRating { get; set; }
        public int CriticRating { get; set; }
        public int CommonRating { get; set; }
        public byte[] ProductPicture { get; set; } = null!;
        public int Year { get; set; }
        public string Country { get; set; }
    }
}
