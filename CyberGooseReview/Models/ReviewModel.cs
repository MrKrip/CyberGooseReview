namespace CyberGooseReview.Models
{
    public class ReviewModel
    {
        public UserDataModel userData { get; set; }
        public int Rating { get; set; }
        public string? Details { get; set; }
        public int Likes { get; set; }
        public int DisLikes { get; set; }
        public DateTime CreationDate { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }
}
