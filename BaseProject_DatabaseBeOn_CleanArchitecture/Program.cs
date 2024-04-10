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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

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

                option.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()); // Resource filter global của thằng ValidateAntiforgeryTokenAttribute => từ giờ nó sẽ tự add không cần phải thủ công add từng attribute làm gì cho cực => ĐIển hình nhất là tại Login action không có add attribute nhưng vẫn có XSRF - Cross Site Request Forgery - CSRF
            });
            builder.Services.AddScoped<ICountriesService, CountriesService>();
            builder.Services.AddScoped<IPersonsService, PersonsService>();
            builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
            builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();

            builder.Services.AddDbContext<BaseProjectDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(option =>
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

            builder.Services.AddAuthorization(option =>
            {
                ///Custom policies
                option.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build(); // Enforeces authorization policy (user must be authenticated) for all action => Khi ta muốn action nào không cần phải đăng nhập mà người dùng vẫn có thể truy cập được thì dùng [AllowAnonymous] attribute ở controller hay action

                option.AddPolicy("NotAuthenticated", policy =>
                {
                    //policy.RequireRole("Admin");//Thằng này người ta đã build sẵn rồi
                    policy.RequireAssertion(context =>
                    {
                        //Thằng này là custom require này
                        
                        return !context.User.Identity.IsAuthenticated; //True là cho access, false thì đéo


                        ///Muốn test thằng custom này thì tăt [AllowAnonymous] attribute tại AccountController vì lỡ cho phép toàn cục anonymous rồi
                    });
                });
            });
            //builder.Services.AddAuthorization(); // bản chất của thằng này là add services với mục đích config Authorization mà thôi, nếu dùng mặc định thì không cần, TH muốn config như thằng ở trên thì dùng
            builder.Services.ConfigureApplicationCookie(option =>
            {
                option.LoginPath = "/Account/Login"; // Kiểu khi người dùng muốn truy cập vào trang nào mà nó yêu cầu phải đăng nhập mà người dùng chưa đăng nhập thì sẽ tự động chuyển về domain trên để đăng nhập
            });


            //StartUp Extention middleware

            //builder.Services.ConfigureServices(builder.Configuration); ////=> Để các service riêng một file khác. => Tạm thời không dùng vì nó sẽ lộn với các note khác :((

            //StartUp Extention middleware


            ///////////////////// Seriolog not need
            //builder.Services.AddHttpLogging(option => {
            //    //option.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All; //  lấy all log về http giống usehttploggin();
            //    option.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProtocol;
            //}); Cái này là built-in logging trong Asp => khi muốn dùng Serilog (Một external log package) thì ta phải xóa cái bult-in. Nói chung chỉ nên xài một cái
            ///////////////////// Seriolog not need
            ///
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
            //use https
            app.UseHsts();
            app.UseHttpsRedirection(); // use https
            app.Logger.LogDebug("Yoyoyo debug"); // thang nay khong hien do default cua asp chi hien tu info cho toi Critical => Muon hien thi phai vao appsettings.json de tu config loglevel => Nho phai check appsetting.Enviroment.json luon neh
            
            //Log trong appsetting co hai dang. Default => La cho nguoi dung tu dinh nghia giong nhu o duoi - Microsoft.AspNetCore => cho cac thu vien da duoc tao san cua asp

            app.Logger.LogInformation("yoyo infor");
            app.Logger.LogWarning("yoyo warning");
            app.Logger.LogError("yoyo error");
            app.Logger.LogCritical("log critical");
            //app.UseHttpLogging(); // lay het tat ca cac log ve http


            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication(); // Reading identity cookie, xem có đăng nhaahjp  hay chưa
            app.UseAuthorization(); // Config page cho( controller action ) nào sẽ được cho phép truy cập khi đã login hay không ------------- Validates access permissions of the user => B2: add authorrization service
            app.MapControllers();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name:"default",
            //        pattern: "{controller}/{action}"
            //        );
            //}); //Convention Routing => kiểu giống như trong mvc cũ ấy, mặc định một template route cho hầu hết controller => Enterprise dùng attribute PS::: Nếu dùng Convention routing mà còn dùng Attribute routing thì thằng Attribute sẽ override thằng Convention
            app.Run();
        }
    }
}
