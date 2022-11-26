using DAL.Context;
using DAL.Entity;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class CatSubCategoryRepository : IRepository<CategoriesSubCategories>
    {
        private DefaultContext db;
        public CatSubCategoryRepository(DefaultContext db)
        {
            this.db = db;
        }

        public void Create(CategoriesSubCategories item)
        {
            db.CategoriesSubCategories.Add(item);
        }

        public void Delete(int id)
        {
            CategoriesSubCategories item = db.CategoriesSubCategories.Find(id);
            if (item != null)
                db.CategoriesSubCategories.Remove(item);
        }

        public IEnumerable<CategoriesSubCategories> Find(Func<CategoriesSubCategories, bool> predicate)
        {
            return db.CategoriesSubCategories.Include(c => c.Category).Include(sc => sc.SubCategory).Where(predicate).ToList();
        }

        public CategoriesSubCategories Get(int id)
        {
            return db.CategoriesSubCategories.Find(id);
        }

        public IEnumerable<CategoriesSubCategories> GetAll()
        {
            return db.CategoriesSubCategories.Include(c => c.SubCategory).Include(sc => sc.SubCategory);
        }

        public void Update(CategoriesSubCategories item)
        {
            db.CategoriesSubCategories.Update(item);
        }
    }
}
