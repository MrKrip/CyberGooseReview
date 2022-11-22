using BLL.DTO;
using DAL.Entity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BLL.Interfaces
{
    public interface IUserService
    {
        public Task<IdentityResult> CreateUser(UserDTO user);
        public Task<IdentityResult> UpdateUser(UserDTO EditUser, ClaimsPrincipal user);
        public Task<IdentityResult> DeleteUser(ClaimsPrincipal user);
        public UserDataDTO GetUserById(string id);
        public IEnumerable<UserDataDTO> GetUsers();
        public IEnumerable<UserDataDTO> FindUsers(Func<User, bool> predicate);
        public Task<bool> IsUserHasveRole(string role, string id);
        Task<IEnumerable<string>> GetUserRoles(string id);
        public Task<bool> LogIn(UserDTO user);
        public Task LogOut();
        public bool IsSignedIn(ClaimsPrincipal user);
        UserDTO GetCurrentUser(ClaimsPrincipal user);
        Task<IdentityResult> CreateNewRole(string role);
        Task<bool> IsCurrentUserHasRole(ClaimsPrincipal user, string role);
        Task<IdentityResult> AddRolesToUser(string id, IEnumerable<string> roles);
        Task<IdentityResult> AddRoleToUser(string id, string roles);
        IEnumerable<IdentityRole> GetAllRoles();
        IEnumerable<IdentityRole> FindRole(Func<IdentityRole, bool> predicate);
        Task<IdentityResult> DeleteRole(string id);
    }
}
