using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DAL.Entity
{
    public class User : IdentityUser
    {
        [Key]
        public int Id { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
