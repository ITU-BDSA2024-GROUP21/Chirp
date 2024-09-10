using System.IO;
using System.Text.RegularExpressions;
using Chirp.CLI;
using System.Globalization;
using CsvHelper;
using System.Collections.Generic;
using SimpleDB;

class Program
{
    public record Cheep(string Author, string Message, long Timestamp);


    public static void Main(string[] args)
    {
        var database = new CSVDatabase();
        if (args.Length > 0)
        {
            switch (args[0])
            {
                case "cheep":
                    cheep.test(args);
                    return;
                case "read":
                    var records = database.Read();
                    foreach (var item in records)
                    {
                        var timestamp = (int)item.Timestamp;
                        var dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp);
                        Console.WriteLine(item.Author + " @ " + dateTime.ToString("MM/dd/yyyy HH:mm:ss") + ": " +
                                          item.Message);
                    }

                    return;
            }
        }
    }
}
    