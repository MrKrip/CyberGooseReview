using DAL.Context;
using DAL.Entity;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class ProductRepository : IRepository<Product>
    {
        private DefaultContext db;
        public ProductRepository(DefaultContext db)
        {
            this.db = db;
        }

        public void Create(Product item)
        {
            db.Products.Add(item);
        }

        public void Delete(int id)
        {
            Product item = db.Products.Find(id);
            if (item != null)
                db.Products.Remove(item);
        }

        public IEnumerable<Product> Find(Func<Product, bool> predicate)
        {
            return db.Products.Include(c => c.Category).Where(predicate).ToList();
        }

        public Product Get(int id)
        {
            return db.Products.Find(id);
        }

        public IEnumerable<Product> GetAll()
        {
            return db.Products.Include(c => c.Category);
        }

        public void Update(Product item)
        {
            db.Products.Update(item);
        }
    }
}
