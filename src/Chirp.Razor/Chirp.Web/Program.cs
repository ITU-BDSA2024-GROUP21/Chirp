using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Repositories;
using Chirp.Infrastructure.Services;


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
        
        builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddEntityFrameworkStores<ChirpDBContext>();
        
        builder.Services.AddScoped<INootRepository, NootRepository>();
        builder.Services.AddScoped<IBioRepository, BioRepository>();
        builder.Services.AddScoped<IFollowRepository, FollowRepository>();
        builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
        builder.Services.AddScoped<INooterService, NooterService>();
        
        builder.Services.AddAuthentication()
            .AddCookie()
            .AddGitHub(o =>
            {
                o.ClientId = builder.Configuration["authentication_github_clientId"] ?? "default-client-id";
                o.ClientSecret = builder.Configuration["authentication_github_clientSecret"] ?? "default-client-secret";
                o.Scope.Add("user:email"); // Makes sure Github sends email & username
                o.Scope.Add("read:user");
                o.CallbackPath = "/signin-github";
            });
        
            

        var app = builder.Build();
        
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
            db.Database.EnsureCreated();
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

