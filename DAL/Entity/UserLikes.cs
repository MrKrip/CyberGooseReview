using System.ComponentModel.DataAnnotations;

namespace DAL.Entity
{
    public class UserLikes
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int ReviewId { get; set; }
        public Review Review { get; set; }
        public bool? Likes { get; set; }
    }
}
