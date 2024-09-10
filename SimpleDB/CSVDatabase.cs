namespace SimpleDB;
using System;
using System.Globalization;
using CsvHelper;

public record Cheep(string Author, string Message, long Timestamp);

public sealed class CSVDatabase : IDatabaseRepository<Cheep>
{   
    public string csvPath = "../SimpleDB/chirp_cli_db.csv";
    public IEnumerable<Cheep> Read(int? limit = null)
    {
        if (limit.HasValue)
        {
            throw new NotImplementedException();
        }

        StreamReader reader = new StreamReader(csvPath); 
        CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture);
    
        {
            var records = csv.GetRecords<Cheep>();
            return records;
        }
    }
    
    public void Store(Cheep record)
    {   
        using (StreamWriter sw = File.AppendText(csvPath))
        {
            sw.WriteLine(record.Author + ",\"" + record.Message + "\"," + record.Timestamp);
        }
    }
}