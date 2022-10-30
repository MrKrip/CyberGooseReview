using DAL.Context;
using DAL.Entity;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class UserLikesRepository : IRepository<UserLikes>
    {
        private DefaultContext db;

        public UserLikesRepository(DefaultContext db)
        {
            this.db = db;
        }

        public void Create(UserLikes item)
        {
            db.UserLikes.Add(item);
        }

        public void Delete(int id)
        {
            UserLikes item = db.UserLikes.Find(id);
            if (item != null)
                db.UserLikes.Remove(item);
        }

        public IEnumerable<UserLikes> Find(Func<UserLikes, bool> predicate)
        {
            return db.UserLikes.Include(r => r.Review).Include(u => u.User).Where(predicate);
        }

        public UserLikes Get(int id)
        {
            return db.UserLikes.Find(id);
        }

        public IEnumerable<UserLikes> GetAll()
        {
            return db.UserLikes.Include(r => r.Review).Include(u => u.User);
        }

        public void Update(UserLikes item)
        {
            db.UserLikes.Update(item);
        }
    }
}
