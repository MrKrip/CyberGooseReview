﻿using DAL.Context;
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
    public class SubCategoryRepository : IRepository<SubCategory>
    {
        private DefaultContext db;
        public SubCategoryRepository(DefaultContext db)
        {
            this.db = db;
        }

        public void Create(SubCategory item)
        {
            db.SubCategories.Add(item);
        }

        public void Delete(int id)
        {
            SubCategory item = db.SubCategories.Find(id);
            if (item != null)
                db.SubCategories.Remove(item);
        }

        public IEnumerable<SubCategory> Find(Func<SubCategory, bool> predicate)
        {
            return db.SubCategories.Include(c => c.Category).Where(predicate).ToList();
        }

        public SubCategory Get(int id)
        {
            return db.SubCategories.Find(id);
        }

        public IEnumerable<SubCategory> GetAll()
        {
            return db.SubCategories.Include(c => c.Category);
        }

        public void Update(SubCategory item)
        {
            db.SubCategories.Update(item);
        }
    }
}
