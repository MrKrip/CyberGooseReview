using System.ComponentModel.DataAnnotations;

namespace CyberGooseReview.Models
{
    public class ProductManageModel
    {
        [Required]
        [Display(Name = "Product name")]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [Display(Name = "Product Picture")]
        public byte[]? ProductPicture { get; set; }
        [Display(Name = "YouTube Link")]
        public string? YouTubeLink { get; set; }
        [Range(1900, 2022, ErrorMessage = "Can only be between 0 .. 2022 ")]
        public int Year { get; set; }
        public string Country { get; set; }
    }
}
