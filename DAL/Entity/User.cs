using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DAL.Entity
{
    public class User : IdentityUser
    {
        public string Salt { get; set; }
        public DateTime? RegistationDate { get; set; }
    }
}
