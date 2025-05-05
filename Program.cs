using System.IdentityModel.Tokens.Jwt;
using System.Text;
using ExaminationSystemMVC.MappingConfig;
using ExaminationSystemMVC.Models;
using ExaminationSystemMVC.Models.JWT;
using ExaminationSystemMVC.Reposatories;
using ExaminationSystemMVC.UnitOfWorks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ExaminationSystemMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<DBContext>(options =>
               options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("con1")));
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddScoped<UnitOfWork>();

            builder.Services.AddScoped<IInstructorRepository, InstructorRepository>();
            builder.Services.AddScoped<UsersRepo>();


            builder.Services.AddHttpContextAccessor();


            builder.Services.AddAuthentication("JwtCookie")
              .AddCookie("JwtCookie", options =>
              {
                  options.LoginPath = "/Account/Login";
                  options.AccessDeniedPath = "/Account/AccessDenied";
              });
            builder.Services.AddScoped<JwtHelper>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.Use(async (context, next) =>
            {
                var token = context.Request.Cookies["jwt"];
                if (!string.IsNullOrEmpty(token))
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

                    try
                    {
                        var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidIssuer = builder.Configuration["Jwt:Issuer"],
                            ValidAudience = builder.Configuration["Jwt:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(key),
                            ClockSkew = TimeSpan.Zero
                        }, out SecurityToken validatedToken);

                        context.User = principal;
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"JWT Validation Failed: {ex.Message}");
                        context.Response.Cookies.Delete("jwt");
                        context.Response.Redirect("/Account/Login");

                    }
                }

                await next();
            });

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
