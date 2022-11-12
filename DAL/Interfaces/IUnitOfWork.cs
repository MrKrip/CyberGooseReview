using DAL.Entity;

namespace DAL.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<Category> Categories { get; }
        IRepository<Product> Products { get; }
        //IRepository<ProductSubCategoreis> ProductSubCategoreis { get; }
        IRepository<Review> Reviews { get; }
        IRepository<SubCategory> SubCategories { get; }
        IRepository<UserLikes> UserLikes { get; }
        IUserRepository<User, UserData> Users { get; }
        void save();
    }
}
