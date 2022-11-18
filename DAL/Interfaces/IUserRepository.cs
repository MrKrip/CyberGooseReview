using Microsoft.AspNetCore.Identity;

namespace DAL.Interfaces
{
    public interface IUserRepository<T, S> where T : class where S : class
    {
        IEnumerable<S> GetAll();
        S Get(string id);
        IEnumerable<S> Find(Func<T, bool> predicate);
        Task<IdentityResult> Create(T item, UserManager<T> userManager);
        void Update(T item);
        void Delete(string id);
        Task LogIn(T item, SignInManager<T> signInManager);
        Task<IdentityResult> AddRolesToUser(string id, IEnumerable<string> roles, UserManager<T> userManager, RoleManager<IdentityRole> roleManager);
        Task<IdentityResult> AddRoleToUser(string id, string roles, UserManager<T> userManager);
        Task<IdentityResult> CreateNewRole(string role, RoleManager<IdentityRole> roleManager);
        IEnumerable<IdentityRole> GetAllRoles(RoleManager<IdentityRole> roleManager);
        IEnumerable<IdentityRole> FindRole(Func<IdentityRole, bool> predicate, RoleManager<IdentityRole> roleManager);
        Task<IdentityResult> DeleteRole(string id, RoleManager<IdentityRole> roleManager);
        Task<IEnumerable<string>> GetUserRoles(string id,UserManager<T> userManager);
    }
}
