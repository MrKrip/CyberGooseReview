using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DAL.Entity
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string ReviewDetails { get; set; } = null!;
        public int Rating { get; set; }
        public int Likes { get; set; }
        public int DisLikes { get; set; }
        public DateTime CreationDate { get; set; }
        public IEnumerable<UserLikes> UserLikes { get; set; }
    }
}
