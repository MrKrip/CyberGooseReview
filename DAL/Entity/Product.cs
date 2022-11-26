using System.ComponentModel.DataAnnotations;

namespace DAL.Entity
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int Year { get; set; }
        public string Country { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? YouTubeLink { get; set; }
        public int UserRating { get; set; }
        public int CriticRating { get; set; }
        public int CommonRating { get; set; }
        public byte[]? ProductPicture { get; set; }
    }
}
