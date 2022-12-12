using BLL.DTO;

namespace CyberGooseReview.Models
{
    public class UserReviewModel
    {
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public string ReviewDetails { get; set; }
        public int Rating { get; set; }
    }
}
