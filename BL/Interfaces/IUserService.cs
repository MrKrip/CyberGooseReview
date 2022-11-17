using BLL.DTO;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BLL.Interfaces
{
    public interface IUserService
    {
        public Task<IdentityResult> CreateUser(UserDTO user);
        public void UpdateUser(UserDTO user);
        public UserDataDTO GetUserById(string id);
        public void DeleteUser(string id);
        public Task<bool> IsUserHasveRole(string role, string id);
        public Task<bool> LogIn(UserDTO user);
        public Task LogOut();
        public bool IsSignedIn(ClaimsPrincipal user);
        UserDTO GetCurrentUser(ClaimsPrincipal user);
    }
}
