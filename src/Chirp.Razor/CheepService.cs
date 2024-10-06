using System.Data;
using Microsoft.Data.Sqlite;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    
    public List<CheepViewModel> GetCheeps();
    public List<CheepViewModel> GetCheepsFromAuthor(string author);
}

public class CheepService : ICheepService
{
    string _sqlDBFilePath = "../chirp.db";
    string sqlQuery = @"SELECT user.username, message.text, message.pub_date FROM message JOIN user ON message.author_id = user.user_id ORDER BY message.pub_date DESC";

    // These would normally be loaded from a database for example
    private static readonly List<CheepViewModel> _cheeps = new();
    

    public List<CheepViewModel> GetCheeps()
    {
        
        using (var connection = new SqliteConnection($"Data Source={_sqlDBFilePath}"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = sqlQuery;
            
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var author = reader.GetString(0);
                var message = reader.GetString(1);
                var timestamp = reader.GetDouble(2);

                var cheep = new CheepViewModel(author, message, UnixTimeStampToDateTimeString(timestamp));
                _cheeps.Add(cheep);
            }
        }
        return _cheeps;
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        // filter by the provided author name
        return _cheeps.Where(x => x.Author == author).ToList();
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

}
