using DAL.Context;
using DAL.Entity;
using DAL.Interfaces;

namespace DAL.Repositories
{
    public class CategoryRepository : IRepository<Category>
    {
        private DefaultContext db;
        public CategoryRepository(DefaultContext db)
        {
            this.db = db;
        }

        public void Create(Category item)
        {
            db.Categories.Add(item);
        }

        public void Delete(int id)
        {
            Category item = db.Categories.Find(id);
            if (item != null)
                db.Categories.Remove(item);
        }

        public IEnumerable<Category> Find(Func<Category, bool> predicate)
        {
            return db.Categories.Where(predicate).ToList();
        }

        public Category Get(int id)
        {
            return db.Categories.Find(id);
        }

        public IEnumerable<Category> GetAll()
        {
            return db.Categories;
        }

        public void Update(Category item)
        {
            db.Categories.Update(item);
        }
    }
}
