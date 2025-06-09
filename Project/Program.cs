using Project.Services;
using Microsoft.EntityFrameworkCore;
using Project.Data;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllersWithViews();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddHostedService<VaccinationReminderService>();
        builder.Services.AddDistributedMemoryCache(); 
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30); 
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
        builder.Configuration
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false)
           .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        builder.Services.AddDbContext<AppDbContext>(options =>
             options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.Configure<EmailSettings>(
            builder.Configuration.GetSection("EmailSettings"));
        builder.Services.AddSingleton<NotificationService>();
        var app = builder.Build();
        using (var scope = app.Services.CreateScope())
        {
            
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            

        }
        using (var scope = app.Services.CreateScope())
        {
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
            if (!authService.Register("admin@example.com", "123456","Ali","0545547851"))
            {
                Console.WriteLine("Admin already exists.");
            }
        }
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseSession(); 
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Account}/{action=Login}/{id?}");

        app.Run();
    }
}
