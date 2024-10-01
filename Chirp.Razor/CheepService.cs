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
    string _sqlDBFilePath = "/../chirp.db";
    string sqlQuery = @"SELECT author, message, pub_date FROM message ORDER BY pub_date DESC";

    // These would normally be loaded from a database for example
    private static readonly List<CheepViewModel> _cheeps = new();
        /*{
            
            new CheepViewModel("Helge", "Hello, BDSA students!", UnixTimeStampToDateTimeString(1690892208)),
            new CheepViewModel("Adrian", "Hej, velkommen til kurset.", UnixTimeStampToDateTimeString(1690895308)),
        }*/
    

    public List<CheepViewModel> GetCheeps()
    {
        //var sqlDBFilePath = "../chirp.db";
        //var sqlQuery = @"SELECT author, message, pub_date JOIN FROM message ORDER BY pub_date DESC";
        
        using (var connection = new SqliteConnection($"Data Source={_sqlDBFilePath}"))
        {
            connection.Open();
            var command = connection.CreateCommand();

            try
            {
                command.CommandText = sqlQuery;

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var author = reader.GetString(0);
                    var message = reader.GetString(1);
                    var timestamp = reader.GetString(2);

                    var cheep = new CheepViewModel(author, message, timestamp);
                    _cheeps.Add(cheep);
                }
            } catch (Exception e) {
                Console.WriteLine(e.Message);
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
