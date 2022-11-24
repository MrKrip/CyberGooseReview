using System.ComponentModel.DataAnnotations;

namespace CyberGooseReview.Models
{
    public class CategoryManageModel
    {
        public int? Id { get; set; }
        [Required]
        public string Name { get; set; }
        public List<SubCatCheckModel> subCategories { get; set; }
    }
}
