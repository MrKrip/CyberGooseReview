using DAL.Context;
using DAL.Entity;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private DefaultContext db;
        private CategoryRepository categoryRepository;
        private ProductRepository productRepository;
        private ProductSubRepository productSubRepository;
        private ReviewRepository reviewRepository;
        private SubCategoryRepository subCategoryRepository;
        private UserRepository userRepository;

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

        public IRepository<ProductSubCategoreis> ProductSubCategoreis
        {
            get
            {
                if (productSubRepository == null)
                    productSubRepository = new ProductSubRepository(db);
                return productSubRepository;
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

        public void save()
        {
            db.SaveChanges();
        }
    }
}
