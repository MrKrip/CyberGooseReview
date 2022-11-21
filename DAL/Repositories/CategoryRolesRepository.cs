using DAL.Context;
using DAL.Entity;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class CategoryRolesRepository : IRepository<CategoryRoles>
    {
        private DefaultContext db;
        public CategoryRolesRepository(DefaultContext db)
        {
            this.db = db;
        }
        public void Create(CategoryRoles item)
        {
            db.CategoryRoles.Add(item);
        }

        public void Delete(int id)
        {
            CategoryRoles item = db.CategoryRoles.Find(id);
            if (item != null)
                db.CategoryRoles.Remove(item);
        }

        public IEnumerable<CategoryRoles> Find(Func<CategoryRoles, bool> predicate)
        {
            return db.CategoryRoles.Include(c => c.Category).Include(r => r.Role).Where(predicate).ToList();
        }

        public CategoryRoles Get(int id)
        {
            return db.CategoryRoles.Find(id);
        }

        public IEnumerable<CategoryRoles> GetAll()
        {
            return db.CategoryRoles.Include(c => c.Category).Include(r => r.Role);
        }

        public void Update(CategoryRoles item)
        {
            db.CategoryRoles.Update(item);
        }
    }
}
