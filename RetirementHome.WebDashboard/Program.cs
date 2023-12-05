using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetirementHome.WebDashboard.Abstractions;
using RetirementHome.WebDashboard.Data;
using RetirementHome.WebDashboard.Hubs;
using System.Text.Json;

namespace RetirementHome.WebDashboard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            builder.Services.AddSignalR();

            builder.Services.AddSingleton<IResidentsRepository, ResidentsRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.MapPost("/resident/new", async (HttpRequest request, IResidentsRepository residentsRepository) =>
            {
                var body = new StreamReader(request.Body);
                string postData = await body.ReadToEndAsync();
                Dictionary<string, dynamic> keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(postData) ?? new Dictionary<string, dynamic>();

                string firstName = keyValuePairs["firstName"].ToString();
                string lastName = keyValuePairs["lastName"].ToString(); 

                var newResident = residentsRepository.AddNewResident(firstName, lastName);

                return await Task.FromResult(newResident);
            });

            app.MapHub<ResidentHub>("/residentHub");

            app.Run();
        }
    }
}