using DAL.Context;
using DAL.Entity;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BLL.Util
{
    public static class ServicesModule
    {

        public static void InitialSevicesDI(string ConnectionString, IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork>(s => new EFUnitOfWork(ConnectionString));

            services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                        .AddEntityFrameworkStores<DefaultContext>();
        }
    }
}
