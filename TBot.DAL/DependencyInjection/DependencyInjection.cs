using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBot.DAL.Repositories;
using TBot.Domain.Entity;
using TBot.Domain.Interfaces.Repositories;

namespace TBot.DAL.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddScoped<IServiceProvider, ServiceProvider>();

            var connectionString = configuration.GetConnectionString("MySQL") ?? "";

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseMySQL(connectionString);
            });

            services.InitRepositories();
        }

        public static void InitRepositories(this IServiceCollection services)
        {
            services.AddTransient<IBaseRepository<UserData>, BaseRepository<UserData>>();
            services.AddTransient<IBaseRepository<UserApp>, BaseRepository<UserApp>>();
            services.AddTransient<IBaseRepository<UserFtp>, BaseRepository<UserFtp>>();
            services.AddTransient<IBaseRepository<UserHistory>, BaseRepository<UserHistory>>();
        }
    }
}
