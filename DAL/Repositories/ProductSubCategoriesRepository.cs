using DAL.Context;
using DAL.Entity;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class ProductSubCategoriesRepository : IRepository<ProductSubCategories>
    {
        private DefaultContext db;
        public ProductSubCategoriesRepository(DefaultContext db)
        {
            this.db = db;
        }

        public void Create(ProductSubCategories item)
        {
            db.ProductSubCategories.Add(item);
        }

        public void Delete(int id)
        {
            ProductSubCategories item = db.ProductSubCategories.Find(id);
            if (item != null)
                db.ProductSubCategories.Remove(item);
        }

        public IEnumerable<ProductSubCategories> Find(Func<ProductSubCategories, bool> predicate)
        {
            return db.ProductSubCategories.Include(sc=>sc.SubCategory).Include(p=>p.Product).Where(predicate).ToList();
        }

        public ProductSubCategories Get(int id)
        {
            return db.ProductSubCategories.Find(id);
        }

        public IEnumerable<ProductSubCategories> GetAll()
        {
            return db.ProductSubCategories;
        }

        public void Update(ProductSubCategories item)
        {
            db.ProductSubCategories.Update(item);
        }
    }
}
