﻿using DAL.Context;
using DAL.Entity;
using DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DAL.Repositories
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private DefaultContext db;
        private CategoryRepository categoryRepository;
        private ProductRepository productRepository;
        private ReviewRepository reviewRepository;
        private SubCategoryRepository subCategoryRepository;
        private UserRepository userRepository;
        private UserLikesRepository userLikesRepository;

        public EFUnitOfWork(string connectionStrng)
        {
            db = new DefaultContext(connectionStrng);            
        }

        public IRepository<Category> Categories
        {
            get
            {
                if (categoryRepository == null)
                    categoryRepository = new CategoryRepository(db);
                return categoryRepository;
            }
        }

        public IRepository<Product> Products
        {
            get
            {
                if (productRepository == null)
                    productRepository = new ProductRepository(db);
                return productRepository;
            }
        }

        public IRepository<Review> Reviews
        {
            get
            {
                if (reviewRepository == null)
                    reviewRepository = new ReviewRepository(db);
                return reviewRepository;
            }
        }

        public IRepository<SubCategory> SubCategories
        {
            get
            {
                if (subCategoryRepository == null)
                    subCategoryRepository = new SubCategoryRepository(db);
                return subCategoryRepository;
            }
        }

        public IUserRepository<User, UserData> Users
        {
            get
            {
                if (userRepository == null)
                    userRepository = new UserRepository(db);
                return userRepository;
            }
        }

        public IRepository<UserLikes> UserLikes
        {
            get
            {
                if (userLikesRepository == null)
                    userLikesRepository = new UserLikesRepository(db);
                return userLikesRepository;
            }
        }

        public void save()
        {
            db.SaveChanges();
        }
    }
}
