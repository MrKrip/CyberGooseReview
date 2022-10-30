using DAL.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.Context
{
    public class DefaultContext : IdentityDbContext
    {
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ProductSubCategoreis> ProductSubCategoreis { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<SubCategory> SubCategories { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<UserLikes> UserLikes { get; set; } = null!;

        public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
