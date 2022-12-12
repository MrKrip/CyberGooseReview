namespace CyberGooseReview.Models
{
    public class ItemWithReviewModel<T>
    {
        public T Item { get; set; }
        public IEnumerable<ReviewModel> Reviews { get; set; }
        public UserReviewModel UserReview { get; set; }
    }
}
