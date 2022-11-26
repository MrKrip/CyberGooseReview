namespace BLL.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public CategoryDTO Category { get; set; }
        public IEnumerable<SubCategoryDTO> SubCategories { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? YouTubeLink { get; set; }
        public int UserRating { get; set; }
        public int CriticRating { get; set; }

        public byte[] ProductPicture { get; set; } = null!;
        public int CommonRating { get; set; }
        public int Year { get; set; }
        public string Country { get; set; }
    }
}
