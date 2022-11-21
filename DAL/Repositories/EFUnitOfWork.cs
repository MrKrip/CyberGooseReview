using DAL.Context;
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
        private CatSubCategoryRepository catSubCategoryRepository;
        private ProductSubCategoriesRepository productSubCategoriesRepository;
        private CategoryRolesRepository categoryRolesRepository;

        public EFUnitOfWork(DbContextOptions<DefaultContext> options)
        {
            db = new DefaultContext(options);
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

        public IRepository<CategoriesSubCategories> CategoriesSubCategories
        {
            get
            {
                if (catSubCategoryRepository == null)
                    catSubCategoryRepository = new CatSubCategoryRepository(db);
                return catSubCategoryRepository;
            }
        }

        public IRepository<ProductSubCategories> ProductSubCategories
        {
            get
            {
                if (productSubCategoriesRepository == null)
                    productSubCategoriesRepository = new ProductSubCategoriesRepository(db);
                return productSubCategoriesRepository;
            }
        }

        public IRepository<CategoryRoles> CategoryRoles
        {
            get
            {
                if (categoryRolesRepository == null)
                    categoryRolesRepository = new CategoryRolesRepository(db);
                return categoryRolesRepository;
            }
        }

        public void save()
        {
            db.SaveChanges();
        }
    }
}
