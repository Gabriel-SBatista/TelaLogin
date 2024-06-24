using Login.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login.Cross.DependencyInjection
{
    public static class SqlServerDependencyInjection
    {
        public static IServiceCollection AddSqlServer(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("LoginSqlServerConnection");

            services.AddDbContext<LoginContext>(options =>
                options.UseSqlServer(connectionString, x => x.MigrationsAssembly("Login.Data"))
            );

            return services;
        }
    }
}
