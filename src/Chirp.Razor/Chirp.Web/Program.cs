using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Chirp.Infrastructure;


public partial class Program
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        var chirpDbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH")
                          ?? Path.Combine(Path.GetTempPath(), "chirp.db");

        
        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddDbContext<ChirpDBContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ChirpDBContext>();
        
        builder.Services.AddScoped<ICheepRepository, CheepRepository>();
        builder.Services.AddScoped<ICheepService, CheepService>();
        
        builder.Services.AddAuthentication()
            .AddCookie()
            .AddGitHub(o =>
            {
                o.ClientId = builder.Configuration["authentication_github_clientId"] ?? throw new InvalidOperationException("Client ID is null");
                o.ClientSecret = builder.Configuration["authentication_github_clientSecret"] ?? throw new InvalidOperationException("Client Secret is null");
                o.CallbackPath = "/signin-github";
            });
        
            

        var app = builder.Build();
        
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
            db.Database.EnsureCreated();
            db.Database.Migrate();
            DbInitializer.SeedDatabase(db);
        }

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapRazorPages();


        app.Run();
    }
}

