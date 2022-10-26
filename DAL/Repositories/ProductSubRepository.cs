using DAL.Context;
using DAL.Entity;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class ProductSubRepository : IRepository<ProductSubCategoreis>
    {
        private DefaultContext db;
        public ProductSubRepository(DefaultContext db)
        {
            this.db = db;
        }

        public void Create(ProductSubCategoreis item)
        {
            db.ProductSubCategoreis.Add(item);
        }

        public void Delete(int id)
        {
            ProductSubCategoreis item = db.ProductSubCategoreis.Find(id);
            if (item != null)
                db.ProductSubCategoreis.Remove(item);
        }

        public IEnumerable<ProductSubCategoreis> Find(Func<ProductSubCategoreis, bool> predicate)
        {
            return db.ProductSubCategoreis.Include(sc => sc.SubCategory).Include(p => p.Product).Where(predicate).ToList();
        }

        public ProductSubCategoreis Get(int id)
        {
            return db.ProductSubCategoreis.Find(id);
        }

        public IEnumerable<ProductSubCategoreis> GetAll()
        {
            return db.ProductSubCategoreis.Include(sc => sc.SubCategory).Include(p => p.Product);
        }

        public void Update(ProductSubCategoreis item)
        {
            db.ProductSubCategoreis.Update(item);
        }
    }
}
