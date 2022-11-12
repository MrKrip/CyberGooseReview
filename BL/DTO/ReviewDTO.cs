namespace BLL.DTO
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public UserDTO User { get; set; }
        public int ProductId { get; set; }
        public ProductDTO Product { get; set; }
        public string ReviewDetails { get; set; }
        public int Rating { get; set; }
        public int Likes { get; set; }
        public int DisLikes { get; set; }
        public DateTime CreationDate { get; set; }

        public IEnumerable<UserLikesDTO> UserLikes { get; set; }
    }
}
