var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/Cheeps", () => new Cheep("me", "Hej!", 1684229348)); //read

app.MapPost("/cheep", (Cheep cheep) => {"Author":"ropf","Message":"Hello, World!", "Timestamp": 1684229348}) //write
    
app.Run();
public record Cheep(string Author, string Message, long Timestamp);