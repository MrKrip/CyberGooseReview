using BLL.DTO;
using Microsoft.AspNetCore.Identity;

namespace BLL.Interfaces
{
    public interface IUserService
    {
        public Task<IdentityResult> CreateUser(UserDTO user);
        public void UpdateUser(UserDTO user);
        public UserDataDTO GetUserById(string id);
        public void DeleteUser(string id);
        public Task<bool> IsUserHasveRole(string role, string id);
        public Task<SignInResult> LogIn(UserDTO user);
        public void LogOut(UserDTO user);
    }
}
