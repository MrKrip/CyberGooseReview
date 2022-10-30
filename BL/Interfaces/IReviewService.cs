using BLL.DTO;

namespace BLL.Interfaces
{
    public interface IReviewService
    {
        public void CreateReview(ReviewDTO review);
        public void UpdateReview(ReviewDTO review);
        public void DeleteReview(int reviewId);
        public void LikeReview(ReviewDTO review);
        public void DisLikeReview(ReviewDTO review);
        public ReviewDTO GetReview(int id);
        public IEnumerable<ReviewDTO> GetAllReviewsToProduct(int productId);
        public IEnumerable<ReviewDTO> GetAllReviewsToProduct(int productId, int pageIndex);
        public IEnumerable<ReviewDTO> GetAllUserReview(string userId);
    }
}
