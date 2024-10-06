namespace Chirp.Razor;

using Microsoft.Data.Sqlite;
public class DBFacade
{
    private string path;
    public DBFacade(string path)
    {
        this.path = path;
    }
    public List<CheepViewModel> CheepQuery(string query, int page, string? Author = null)
    {
        List<CheepViewModel> cheeps = new List<CheepViewModel>();
        using (var connection = new SqliteConnection($"Data Source={path}"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@page", page);
            if (Author != null)
            {
                command.Parameters.AddWithValue("@author", Author);
            }
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var author = reader.GetString(0);
                var message = reader.GetString(1);
                var timestamp = reader.GetDouble(2);
                var cheep = new CheepViewModel(author, message, CheepService.UnixTimeStampToDateTimeString(timestamp));
                cheeps.Add(cheep);
            }
        }
        return cheeps;
    }
}