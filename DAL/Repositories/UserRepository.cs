using DAL.Context;
using DAL.Entity;
using DAL.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DAL.Repositories
{
    public class UserRepository : IUserRepository<User, UserData>
    {
        private DefaultContext db;

        public UserRepository(DefaultContext db)
        {
            this.db = db;
        }

        public async Task<IdentityResult> AddRolesToUser(string id, IEnumerable<string> roles, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            var user = await userManager.FindByIdAsync(id);
            var userRoles = await userManager.GetRolesAsync(user);
            var allRoles = roleManager.Roles.ToList();
            var addedRoles = roles.Except(userRoles);
            var removedRoles = userRoles.Except(roles);
            var result = await userManager.AddToRolesAsync(user, addedRoles);
            await userManager.RemoveFromRolesAsync(user, removedRoles);
            return result;
        }

        public async Task<IdentityResult> AddRoleToUser(string id, string roles, UserManager<User> userManager)
        {
            return await userManager.AddToRoleAsync(await userManager.FindByIdAsync(id), roles);
        }

        public async Task<IdentityResult> Create(User item, UserManager<User> userManager)
        {
            return await userManager.CreateAsync(item);
        }

        public async Task<IdentityResult> CreateNewRole(string role, RoleManager<IdentityRole> roleManager)
        {
            var newRole = new IdentityRole();
            newRole.Name = role;
            return await roleManager.CreateAsync(newRole);
        }

        public async Task<IdentityResult> Delete(User item, UserManager<User> userManager)
        {
            return await userManager.DeleteAsync(item);
        }

        public async Task<IdentityResult> DeleteRole(string id, RoleManager<IdentityRole> roleManager)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            return await roleManager.DeleteAsync(role);
        }

        public IEnumerable<UserData> Find(Func<User, bool> predicate)
        {
            return db.Users.Where(predicate).Select(u => new UserData
            {
                Id = u.Id,
                UserNick = u.UserNick,
                UserName = u.UserName,
                Email = u.Email,
                ProfilePicture = u.ProfilePicture,
                RegistationDate = u.RegistationDate,
                Salt = u.Salt,
                Tag = u.Tag
            }).ToList();
        }

        public IEnumerable<IdentityRole> FindRole(Func<IdentityRole, bool> predicate, RoleManager<IdentityRole> roleManager)
        {
            return roleManager.Roles.ToList().Where(predicate);
        }

        public UserData Get(string id)
        {
            var u = db.Users.Find(id);
            return new UserData { Id = u.Id, UserName = u.UserName };
        }

        public IEnumerable<UserData> GetAll()
        {
            return db.Users.Select(u => new UserData
            {
                Id = u.Id,
                UserNick = u.UserNick,
                UserName = u.UserName,
                Email = u.Email,
                ProfilePicture = u.ProfilePicture,
                RegistationDate = u.RegistationDate,
                Salt = u.Salt,
                Tag = u.Tag
            }).ToList();
        }

        public IEnumerable<IdentityRole> GetAllRoles(RoleManager<IdentityRole> roleManager)
        {
            return roleManager.Roles.ToList();
        }

        public async Task<IEnumerable<string>> GetUserRoles(string id, UserManager<User> userManager)
        {
            return await userManager.GetRolesAsync(await userManager.FindByIdAsync(id));
        }

        public async Task LogIn(User item, SignInManager<User> signInManager)
        {
            await signInManager.SignInAsync(item, false);
        }

        public async Task<IdentityResult> Update(User item, UserManager<User> userManager)
        {
            return await userManager.UpdateAsync(item);
        }
    }
}
