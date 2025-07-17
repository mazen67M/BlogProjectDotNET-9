using BlogProjectDotNET_9.Data;
using BlogProjectDotNET_9.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BlogProjectDotNET_9
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("constr"));
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddAuthentication().AddCookie();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath= "/Account/Login";
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                string[] roles = { "Admin", "Author", "User" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }


                var admin = await userManager.FindByEmailAsync("mazenmohsen11111@gmail.com");
                if (admin == null)
                {
                    var newAdmin = new ApplicationUser
                    {
                        UserName = "admin",
                        Email = "mazenmohsen11111@gmail.com",
                        FullName = "Mazen Admin",
                        EmailConfirmed = true,
                        IsApproved = true,
                    };
                    await userManager.CreateAsync(newAdmin, "Mazen123@");
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }

                app.UseHttpsRedirection();
                app.UseStaticFiles();
                app.UseRouting();

                app.UseAuthentication();
                app.UseAuthorization();

                app.MapStaticAssets();
                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}")
                    .WithStaticAssets();
                app.Run();
            }
        }
    }
