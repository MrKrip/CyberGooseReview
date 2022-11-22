using System.ComponentModel.DataAnnotations;

namespace CyberGooseReview.Models
{
    public class UserManageModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserNick { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Display(Name = "Profile Picture")]
        public byte[]? ProfilePicture { get; set; }
    }
}
