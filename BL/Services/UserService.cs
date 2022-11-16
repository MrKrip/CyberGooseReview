using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using BLL.Security;
using DAL.Entity;
using DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
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
            var config = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDTO>());
            var mapper = new Mapper(config);
            string password = Make_MD5.GetHash(user.Password);
            var salt = new byte[AesGcm.NonceByteSizes.MaxSize];
            var tag = new byte[AesGcm.TagByteSizes.MaxSize];
            user.Password = AES_GCM.Encrypt(password, _config, salt, tag);
            user.Salt = Convert.ToBase64String(salt);
            user.Tag = Convert.ToBase64String(tag);
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
            var config = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDataDTO>());
            var mapper = new Mapper(config);
            return await _userManager.IsInRoleAsync(mapper.Map<User>(GetUserById(id)), role);
        }

        public async Task<SignInResult> LogIn(UserDTO user)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<UserData, UserDataDTO>());
            var mapper = new Mapper(config);
            var UserData = DataBase.Users.Get(user.Id);
            user.Password = AES_GCM.Encrypt(Make_MD5.GetHash(user.Password), _config, Convert.FromBase64String(UserData.Salt), Convert.FromBase64String(UserData.Tag));
            return await DataBase.Users.LogIn(mapper.Map<User>(user), _signInManager);
        }

        public void UpdateUser(UserDTO user)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<UserData, UserDataDTO>());
            var mapper = new Mapper(config);
            DataBase.Users.Update(mapper.Map<User>(user));
            DataBase.save();
        }

        public async void LogOut(UserDTO user)
        {
            await _signInManager.SignOutAsync();
        }
    }
}
