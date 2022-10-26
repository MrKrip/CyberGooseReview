using Microsoft.AspNetCore.Identity;

namespace DAL.Entity
{
    public class User : IdentityUser
    {
        public DateTime RegistrationDate { get; set; }
    }
}
