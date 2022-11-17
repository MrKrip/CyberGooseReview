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

        public async Task<IdentityResult> Create(User item, UserManager<User> userManager)
        {
            return await userManager.CreateAsync(item);
        }

        public void Delete(string id)
        {
            User user = db.Users.Find(id);
            if (user != null)
                db.Users.Remove(user);
        }

        public IEnumerable<UserData> Find(Func<User, bool> predicate)
        {
            return db.Users.Where(predicate).Select(u => new UserData
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                ProfilePicture = u.ProfilePicture,
                RegistationDate = u.RegistationDate,
                Salt = u.Salt,
                Tag = u.Tag
            }).ToList();
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
                UserName = u.UserName,
                Email = u.Email,
                ProfilePicture = u.ProfilePicture,
                RegistationDate = u.RegistationDate,
                Salt = u.Salt,
                Tag = u.Tag
            }).ToList();
        }

        public async Task LogIn(User item, SignInManager<User> signInManager)
        {
            await signInManager.SignInAsync(item, false);
        }

        public void Update(User item)
        {
            db.Users.Update(item);
        }
    }
}
