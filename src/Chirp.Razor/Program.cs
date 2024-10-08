using Chirp.Razor;


public partial class Program
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        var chirpDbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH")
                          ?? Path.Combine(Path.GetTempPath(), "chirp.db");


        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddSingleton<CheepService>();
        builder.Services.AddTransient<DBFacade>(_ => new DBFacade(chirpDbPath)
        {
            Author = null,
            Message = null,
            Timestamp = 0
        });
        builder.Services.AddScoped<ICheepRepository, CheepRepository>();



        var app = builder.Build();

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

