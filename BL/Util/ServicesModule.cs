using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BLL.Util
{
    public static class ServicesModule
    {

        public static void InitialSevicesDI(string ConnectionString, IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork>(s => new EFUnitOfWork(ConnectionString));
        }
    }
}
