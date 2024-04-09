using BaseProject_DatabaseBeOn.Filters;
using BaseProjectServices;
using CoreLayer.Domain.IdentityEntities;
using Entities;
using Microsoft.AspNetCore.Authorization;
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
            
            services.AddIdentity<ApplicationUser, ApplicationRole>(option =>
            {
                ////// Khúc này là để config một số thứ như password liên quan đến validation trước khi lưu vào database identity
                ///
                option.Password.RequireUppercase = false;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequiredLength = 5;
            }) // Add identity Services
                .AddEntityFrameworkStores<BaseProjectDbContext>() // DBcontext nao Store Data identity
                .AddDefaultTokenProviders() // for reset password, quen mat khau, validation v.v va , Confirm, otp ...v.v va may may cai nay la can thiet cho security
                .AddUserStore<UserStore<ApplicationUser, ApplicationRole, BaseProjectDbContext, Guid>>() // Repository about user, cannot used directly with dbcontext
                .AddRoleStore<RoleStore<ApplicationRole, BaseProjectDbContext, Guid>>(); //same witt cai tren

            services.AddAuthorization(option =>
            {
                option.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build(); // Enforeces authorization policy (user must be authenticated) for all action => Khi ta muốn action nào không cần phải đăng nhập mà người dùng vẫn có thể truy cập được thì dùng [AllowAnonymous] attribute ở controller hay action
            });
            //builder.Services.AddAuthorization(); // bản chất của thằng này là add services với mục đích config Authorization mà thôi, nếu dùng mặc định thì không cần, TH muốn config như thằng ở trên thì dùng
            services.ConfigureApplicationCookie(option =>
            {
                option.LoginPath = "/Account/Login"; // Kiểu khi người dùng muốn truy cập vào trang nào mà nó yêu cầu phải đăng nhập mà người dùng chưa đăng nhập thì sẽ tự động chuyển về domain trên để đăng nhập
            });




            return services;
        }
    }
}
