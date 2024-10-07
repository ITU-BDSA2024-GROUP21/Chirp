using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Data.Sqlite;

namespace Chirp.Razor;

public class DBFacade
{
    private string _path;
    public DBFacade(string path)
    {
        this._path = path;
        if (path == Path.Combine(Path.GetTempPath(), "chirp.db"))
        {
            InitDB();
        }
    }
    public List<CheepViewModel> CheepQuery(string query, int page, string? Author = null)
    {
        List<CheepViewModel> cheeps = new List<CheepViewModel>();
        using (var connection = new SqliteConnection($"Data Source={_path}"))
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

    private void InitDB()
    {
        var schema = ReadSql("data/schema.sql");
        var dump = ReadSql("data/dump.sql");
            
        RunSql(schema);
        RunSql(dump);
    }

    private string ReadSql(string sqlPath)
    {
        var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
        
        using var DBReader = embeddedProvider.GetFileInfo(sqlPath).CreateReadStream();
        using var DBStreamReader = new StreamReader(DBReader);
        var table = DBStreamReader.ReadToEnd();

        return table;
    }

    private void RunSql(string query)
    {
        using var connection = new SqliteConnection($"Data Source={_path}");
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.ExecuteNonQuery();
    }
}