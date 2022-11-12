using System.ComponentModel.DataAnnotations;

namespace DAL.Entity
{
    public class UserLikes
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public bool? Likes { get; set; }
    }
}
