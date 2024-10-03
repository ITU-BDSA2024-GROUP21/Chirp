using Chirp.Razor;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();



string path = "../chirp.db";
/*
if (Environment.GetEnvironmentVariable("CHIRPDBPATH") != null)
{
    path = Environment.GetEnvironmentVariable("CHIRPDBPATH");
} else{
    path = Path.Combine("C:\\Users\\krist\\RiderProjects\\Chirp\\chirp.db");
    Console.WriteLine(path);
}
if (!File.Exists(path))
{
    newDatabase(path);
}
*/

builder.Services.AddSingleton<DBFacade>(_ => new DBFacade(path));

void newDatabase(string path)
{
    using var connection = new SqliteConnection($"Data Source={path}");
    connection.Open();

    var command = connection.CreateCommand();
    command.CommandText = @"

    create table if not exists user (
        user_id integer primary key autoincrement,
        username string not null,
        email string not null,
        pw_hash string not null
                                      );
    create table if not exists message (
        message_id integer primary key autoincrement,
        author_id integer not null,
        text string not null,
        pub_date integer
                                    
    );";
    command.ExecuteNonQuery();
}

builder.Services.AddSingleton<ICheepService, CheepService>();

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
