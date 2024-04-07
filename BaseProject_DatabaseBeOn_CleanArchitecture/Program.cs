using Microsoft.EntityFrameworkCore;
using Entities;
using BaseProjectServices;
using ServiceContract;
using Repositories;
using RepositoryContracts;
using BaseProject_DatabaseBeOn.Filters;
using BaseProject_DatabaseBeOn.StartUpExtension;
using BaseProject_DatabaseBeOn.Middleware;
using CoreLayer.Domain.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BaseProject_DatabaseBeOn
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //Logging
            //builder.Host.ConfigureLogging(loggingProvider =>
            //{
            //    loggingProvider.ClearProviders(); // Khong hien tat ca log
            //    loggingProvider.AddDebug();// add lai log Debug
            //});
            //
            builder.Services.AddControllersWithViews(option =>
            {
                //option.Filters.Add<ResponeHeaderActionFilter>(); // cai nay khong the supply parameter => add by type
                var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<ResponeHeaderActionFilter>>(); // Lấy service instance
                option.Filters.Add(new ResponeHeaderActionFilter(logger, "Entire-Key", "Entire-Value",2)); // add by Instance
                //
            });
            builder.Services.AddScoped<ICountriesService, CountriesService>();
            builder.Services.AddScoped<IPersonsService, PersonsService>();
            builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
            builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();

            builder.Services.AddDbContext<BaseProjectDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>() // Add identity Services
                .AddEntityFrameworkStores<BaseProjectDbContext>() // DBcontext nao Store Data identity
                .AddDefaultTokenProviders() // for reset password, quen mat khau, validation v.v va , Confirm, otp ...v.v va may may cai nay la can thiet cho security
                .AddUserStore<UserStore<ApplicationUser, ApplicationRole, BaseProjectDbContext, Guid>>() // Repository about user, cannot used directly with dbcontext
                .AddRoleStore<RoleStore<ApplicationRole, BaseProjectDbContext, Guid>>(); //same witt cai tren
                                                                                         //StartUp Extention middleware

            //builder.Services.ConfigureServices(builder.Configuration); ////=> Để các service riêng một file khác. => Tạm thời không dùng vì nó sẽ lộn với các note khác :((

            //StartUp Extention middleware

            //builder.Services.AddHttpLogging(option => {
            //    //option.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All; //  lấy all log về http giống usehttploggin();
            //    option.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProtocol;
            //}); Cái này là built-in logging trong Asp => khi muốn dùng Serilog (Một external log package) thì ta phải xóa cái bult-in. Nói chung chỉ nên xài một cái

            var app = builder.Build();
            if (builder.Environment.IsDevelopment())

            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error"); // Chạy trước để catch thằng ở dưới
                app.UseExceptionHandlingMiddleware(); // Thằng này sẽ catch excep từ Action trước, do nó gần các middleware kế tiếp hơn
            }

            app.Logger.LogDebug("Yoyoyo debug"); // thang nay khong hien do default cua asp chi hien tu info cho toi Critical => Muon hien thi phai vao appsettings.json de tu config loglevel => Nho phai check appsetting.Enviroment.json luon neh
            
            //Log trong appsetting co hai dang. Default => La cho nguoi dung tu dinh nghia giong nhu o duoi - Microsoft.AspNetCore => cho cac thu vien da duoc tao san cua asp

            app.Logger.LogInformation("yoyo infor");
            app.Logger.LogWarning("yoyo warning");
            app.Logger.LogError("yoyo error");
            app.Logger.LogCritical("log critical");
            //app.UseHttpLogging(); // lay het tat ca cac log ve http


            app.UseStaticFiles();
            app.UseRouting();
            app.MapControllers();
            app.Run();
        }
    }
}
