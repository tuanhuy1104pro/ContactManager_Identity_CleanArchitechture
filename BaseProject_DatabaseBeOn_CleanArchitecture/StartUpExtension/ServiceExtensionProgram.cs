using BaseProject_DatabaseBeOn.Filters;
using BaseProjectServices;
using CoreLayer.Domain.IdentityEntities;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContract;

namespace BaseProject_DatabaseBeOn.StartUpExtension
{
    public static class ServiceExtensionProgram
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddControllersWithViews(option =>
            {
                //option.Filters.Add<ResponeHeaderActionFilter>(); // cai nay khong the supply parameter => add by type
                var logger = services.BuildServiceProvider().GetRequiredService<ILogger<ResponeHeaderActionFilter>>(); // Lấy service instance
                option.Filters.Add(new ResponeHeaderActionFilter(logger, "Entire-Key", "Entire-Value", 2)); // add by Instance
                //
            });
            services.AddScoped<ICountriesService, CountriesService>();
            services.AddScoped<IPersonsService, PersonsService>();
            services.AddScoped<ICountriesRepository, CountriesRepository>();
            services.AddScoped<IPersonsRepository, PersonsRepository>();

            ////------------------Enable Identity in this project
            services.AddIdentity<ApplicationUser, ApplicationRole>() // Add identity Services
                .AddEntityFrameworkStores<BaseProjectDbContext>() // DBcontext nao Store Data identity
                .AddDefaultTokenProviders() // for reset password, quen mat khau, validation v.v va , Confirm, otp ...v.v va may may cai nay la can thiet cho security
                .AddUserStore<UserStore<ApplicationUser, ApplicationRole, BaseProjectDbContext, Guid>>() // Repository aboit user, cannot used directly with dbcontext
                .AddRoleStore<RoleStore<ApplicationRole,BaseProjectDbContext,Guid>>(); //same witt cai tren
                //Nôm na phải add đủ các cấu trúc để nó chạy ổn định
                 
            //Store identity tại Dbcontext là BaseProjectDbContext

            ///////////////
            services.AddDbContext<BaseProjectDbContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
            return services;
        }
    }
}
