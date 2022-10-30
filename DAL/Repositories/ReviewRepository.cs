using DAL.Context;
using DAL.Entity;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class ReviewRepository : IRepository<Review>
    {
        private DefaultContext db;
        public ReviewRepository(DefaultContext db)
        {
            this.db = db;
        }

        public void Create(Review item)
        {
            db.Reviews.Add(item);
        }

        public void Delete(int id)
        {
            Review item = db.Reviews.Find(id);
            if (item != null)
                db.Reviews.Remove(item);
        }

        public IEnumerable<Review> Find(Func<Review, bool> predicate)
        {
            return db.Reviews.Include(u => u.User).Include(p => p.Product).Where(predicate).ToList();
        }

        public Review Get(int id)
        {
            return db.Reviews.Find(id);
        }

        public IEnumerable<Review> GetAll()
        {
            return db.Reviews.Include(u => u.User).Include(p => p.Product);
        }

        public void Update(Review item)
        {
            db.Reviews.Update(item);
        }
    }
}
