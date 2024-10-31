using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


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
        
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "GitHub";
            })
            .AddCookie()
            .AddGitHub(o =>
            {
                o.ClientId = builder.Configuration["authentication:github:clientId"] ?? throw new InvalidOperationException("Client ID is null");
                o.ClientSecret = builder.Configuration["authentication:github:clientSecret"] ?? throw new InvalidOperationException("Client Secret is null");
                o.CallbackPath = "/signin-github";
            });
        
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromSeconds(1800);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
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
        app.UseSession();
        
        app.MapRazorPages();


        app.Run();
    }
}

