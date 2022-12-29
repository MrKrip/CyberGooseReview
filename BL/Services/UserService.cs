using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using BLL.Security;
using DAL.Entity;
using DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Security.Cryptography;

namespace BLL.Services
{
    public class UserService : IUserService
    {
        private IUnitOfWork DataBase;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        public UserService(IUnitOfWork db, UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config, RoleManager<IdentityRole> roleManager)
        {
            DataBase = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> CreateUser(UserDTO user)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<UserDTO, User>());
            var mapper = new Mapper(config);
            string password = Make_MD5.GetHash(user.PasswordHash);
            var salt = new byte[AesGcm.NonceByteSizes.MaxSize];
            var tag = new byte[AesGcm.TagByteSizes.MaxSize];
            user.PasswordHash = AES_GCM.Encrypt(password, _config, salt, tag);
            user.Salt = Convert.ToBase64String(salt);
            user.Tag = Convert.ToBase64String(tag);
            user.RegistationDate = DateTime.Now;
            user.Id = Activator.CreateInstance<User>().Id;
            user.UserName = user.Email;
            user.ProfilePicture = File.ReadAllBytes(Directory.GetCurrentDirectory() + @"\wwwroot\img\Goose.jpg");
            var newUser = mapper.Map<User>(user);
            var result = await DataBase.Users.Create(newUser, _userManager);
            DataBase.save();
            return result;
        }

        public UserDataDTO GetUserById(string id)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<UserData, UserDataDTO>());
            var mapper = new Mapper(config);
            return mapper.Map<UserDataDTO>(DataBase.Users.Get(id));
        }

        public async Task<bool> IsUserHasveRole(string id, string role)
        {
            return await _userManager.IsInRoleAsync(await _userManager.FindByIdAsync(id), role);
        }

        public async Task<bool> IsCurrentUserHasRole(ClaimsPrincipal user, string role)
        {
            return await _userManager.IsInRoleAsync(await _userManager.GetUserAsync(user), role);
        }

        public async Task<bool> LogIn(UserDTO user)
        {
            var UserData = await _userManager.FindByEmailAsync(user.Email);
            if (UserData == null)
            {
                return false;
            }
            user.PasswordHash = AES_GCM.Encrypt(Make_MD5.GetHash(user.PasswordHash), _config, Convert.FromBase64String(UserData.Salt), Convert.FromBase64String(UserData.Tag));
            if (UserData.PasswordHash == user.PasswordHash)
            {
                await DataBase.Users.LogIn(UserData, _signInManager);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<IdentityResult> UpdateUser(UserDTO EditUser, ClaimsPrincipal user)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<UserDTO, User>());
            var mapper = new Mapper(config);
            var NewUser = await _userManager.GetUserAsync(user);
            NewUser.UserNick = EditUser.UserNick;
            NewUser.UserName = EditUser.Email;
            NewUser.Email = EditUser.Email;
            if (EditUser.ProfilePicture != null && EditUser.ProfilePicture.Length > 0)
            {
                NewUser.ProfilePicture = EditUser.ProfilePicture;
            }
            var result = await DataBase.Users.Update(mapper.Map<User>(NewUser), _userManager);
            DataBase.save();
            return result;
        }

        public async Task LogOut()
        {
            await _signInManager.SignOutAsync();
        }

        public bool IsSignedIn(ClaimsPrincipal user)
        {
            return _signInManager.IsSignedIn(user);
        }

        public UserDTO GetCurrentUser(ClaimsPrincipal user)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDTO>());
            var mapper = new Mapper(config);
            return mapper.Map<UserDTO>(_userManager.GetUserAsync(user).Result);
        }

        public async Task<IdentityResult> CreateNewRole(string role)
        {

            return await DataBase.Users.CreateNewRole(role, _roleManager);
        }

        public async Task<IdentityResult> AddRolesToUser(string id, IEnumerable<string> roles)
        {
            return await DataBase.Users.AddRolesToUser(id, roles, _userManager, _roleManager);
        }

        public async Task<IdentityResult> AddRoleToUser(string id, string roles)
        {
            return await DataBase.Users.AddRoleToUser(id, roles, _userManager);
        }

        public IEnumerable<IdentityRole> GetAllRoles()
        {
            return DataBase.Users.GetAllRoles(_roleManager);
        }

        public IEnumerable<IdentityRole> FindRole(Func<IdentityRole, bool> predicate)
        {
            return DataBase.Users.FindRole(predicate, _roleManager);
        }

        public async Task<IdentityResult> DeleteRole(string id)
        {
            return await DataBase.Users.DeleteRole(id, _roleManager);
        }

        public IEnumerable<UserDataDTO> GetUsers()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<UserData, UserDataDTO>());
            var mapper = new Mapper(config);
            return DataBase.Users.GetAll().Select(ud => mapper.Map<UserDataDTO>(ud));
        }

        public IEnumerable<UserDataDTO> FindUsers(Func<User, bool> predicate)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<UserData, UserDataDTO>());
            var mapper = new Mapper(config);
            return DataBase.Users.Find(predicate).Select(ud => mapper.Map<UserDataDTO>(ud));
        }

        public async Task<IEnumerable<string>> GetUserRoles(string id)
        {
            return await DataBase.Users.GetUserRoles(id, _userManager);
        }

        public async Task<IdentityResult> DeleteUser(ClaimsPrincipal user)
        {
            return await DataBase.Users.Delete(await _userManager.GetUserAsync(user), _userManager);
        }

        public async Task<IEnumerable<string>> CriticRoles(string UserId, int CatId)
        {
            List<string> roles = new List<string>();
            var CatRoles = DataBase.CategoryRoles.Find(cr => cr.CategoryId == CatId);
            var user = await _userManager.FindByIdAsync(UserId);
            foreach (var role in CatRoles)
            {
                var roleName = (await _roleManager.FindByIdAsync(role.RoleID)).Name;
                if (await _userManager.IsInRoleAsync(user, roleName))
                {
                    roles.Add(roleName);
                }
            }
            return roles;
        }
    }
}
