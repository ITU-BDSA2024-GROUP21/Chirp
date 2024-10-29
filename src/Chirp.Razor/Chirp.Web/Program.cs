using Microsoft.EntityFrameworkCore;


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
        
        builder.Services.AddScoped<ICheepRepository, CheepRepository>();
        builder.Services.AddScoped<ICheepService, CheepService>();

        var app = builder.Build();
        
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
            //db.Database.Migrate();
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

        app.MapRazorPages();


        app.Run();
    }
}

