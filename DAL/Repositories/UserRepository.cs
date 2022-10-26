using DAL.Context;
using DAL.Entity;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace DAL.Repositories
{
    public class UserRepository : IUserRepository<User, UserData>
    {
        public DefaultContext db;
        public UserRepository(DefaultContext db)
        {
            this.db = db;
        }

        public void Create(User item)
        {
            db.Users.Add(item);
        }

        public void Delete(string id)
        {
            User user = db.Users.Find(id);
            if (user != null)
                db.Users.Remove(user);
        }

        public IEnumerable<UserData> Find(Func<User, bool> predicate)
        {
            return db.Users.Where(predicate).Select(u => new UserData { Id = u.Id, Name = u.UserName, RegistrationDate = u.RegistrationDate }).ToList();
        }

        public UserData Get(string id)
        {
            var u = db.Users.Find(id);
            return new UserData { Id = u.Id, Name = u.UserName, RegistrationDate = u.RegistrationDate };
        }

        public IEnumerable<UserData> GetAll()
        {
            return db.Users.Select(u => new UserData { Id = u.Id, Name = u.UserName, RegistrationDate = u.RegistrationDate }).ToList();
        }

        public void Update(User item)
        {
            db.Users.Update(item);
        }
    }
}
