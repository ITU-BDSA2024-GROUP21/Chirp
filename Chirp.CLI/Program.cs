using System.IO;
using System.Text.RegularExpressions;
using Chirp.CLI;
using System.Globalization;
using CsvHelper;
using System.Collections.Generic;

class Program {
    public record Cheep(string Author, string Message, long Timestamp);
    
    public static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            switch (args[0])
            {
                case "cheep":
                    cheep.test(args);
                    return;
                case "read":
                    readCSV(args);
                    return;
            }
        }
    }

    static void readCSV(string[] args)

    {   
        using (var reader = new StreamReader("chirp_cli_db.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csv.GetRecords<Cheep>();
            foreach (var item in records)
            {
                var timestamp = (int) item.Timestamp;
                var dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp);
                Console.WriteLine(item.Author + " @ " + dateTime.ToString("MM/dd/yyyy HH:mm:ss") + ": " + item.Message);
            }
        }
    }
}
