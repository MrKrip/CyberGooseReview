using DAL.Context;
using DAL.Entity;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BLL.Util
{
    public static class ServicesModule
    {

        public static void InitialSevicesDI(string ConnectionString, IServiceCollection services)
        {
            services.AddDbContext<DefaultContext>(options => options.UseSqlServer(ConnectionString));

            var optionsBuilder = new DbContextOptionsBuilder<DefaultContext>();

            services.AddTransient<IUnitOfWork>(s => new EFUnitOfWork(optionsBuilder.UseSqlServer(ConnectionString).Options));

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<DefaultContext>();
        }

    }
}
