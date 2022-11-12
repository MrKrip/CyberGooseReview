﻿using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using DAL.Entity;
using DAL.Interfaces;

namespace BLL.Services
{
    public class UserService : IUserService
    {
        private IUnitOfWork DataBase;
        public UserService(IUnitOfWork db)
        {
            DataBase = db;
        }

        public void CreateUser(UserDTO user)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDTO>());
            var mapper = new Mapper(config);
            DataBase.Users.Create(mapper.Map<User>(user));
            DataBase.save();
        }

        public void DeleteUser(int id)
        {
            DataBase.Users.Delete(id);
            DataBase.save();
        }

        public UserDataDTO GetUserById(int id)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<UserData, UserDataDTO>());
            var mapper = new Mapper(config);
            return mapper.Map<UserDataDTO>(DataBase.Users.Get(id));
        }

        public bool IsUserHasveRole(string role, int id)
        {
            throw new NotImplementedException();
        }

        public void LogIn(UserDTO user)
        {
            throw new NotImplementedException();
        }

        public void UpdateUser(UserDTO user)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<UserData, UserDataDTO>());
            var mapper = new Mapper(config);
            DataBase.Users.Update(mapper.Map<User>(user));
            DataBase.save();
        }
    }
}
