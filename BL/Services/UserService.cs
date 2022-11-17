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
        private readonly IConfiguration _config;
        public UserService(IUnitOfWork db, UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config)
        {
            DataBase = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
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
            var newUser = mapper.Map<User>(user);
            var result = await DataBase.Users.Create(mapper.Map<User>(user), _userManager);
            DataBase.save();
            return result;
        }

        public void DeleteUser(string id)
        {
            DataBase.Users.Delete(id);
            DataBase.save();
        }

        public UserDataDTO GetUserById(string id)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<UserData, UserDataDTO>());
            var mapper = new Mapper(config);
            return mapper.Map<UserDataDTO>(DataBase.Users.Get(id));
        }

        public async Task<bool> IsUserHasveRole(string role, string id)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<UserDataDTO, User>());
            var mapper = new Mapper(config);
            return await _userManager.IsInRoleAsync(mapper.Map<User>(GetUserById(id)), role);
        }

        public async Task<bool> LogIn(UserDTO user)
        {
            var UserData = await _userManager.FindByEmailAsync(user.Email);
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

        public void UpdateUser(UserDTO user)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<UserDTO, User>());
            var mapper = new Mapper(config);
            DataBase.Users.Update(mapper.Map<User>(user));
            DataBase.save();
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
    }
}
