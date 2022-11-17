using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DAL.Entity
{
    public class User : IdentityUser
    {
        public string UserNick { get; set; }
        public string Salt { get; set; }
        public string Tag { get; set; }
        public DateTime? RegistationDate { get; set; }
        public byte[] ProfilePicture { get; set; } = null!;
    }
}
