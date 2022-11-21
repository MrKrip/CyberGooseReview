using DAL.Entity;
using Microsoft.AspNetCore.Identity;

namespace DAL.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<Category> Categories { get; }
        IRepository<Product> Products { get; }
        IRepository<Review> Reviews { get; }
        IRepository<SubCategory> SubCategories { get; }
        IRepository<UserLikes> UserLikes { get; }
        IRepository<CategoriesSubCategories> CategoriesSubCategories { get; }
        IRepository<ProductSubCategories> ProductSubCategories { get; }
        IRepository<CategoryRoles> CategoryRoles { get; }
        IUserRepository<User, UserData> Users { get; }
        void save();
    }
}
