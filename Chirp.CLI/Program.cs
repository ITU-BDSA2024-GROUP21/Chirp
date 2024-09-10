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
                    Console.WriteLine(item);
            }
            
            

            /*
            {

                List<String> authors = new List<String>();

                List<String> cheeps = new List<String>();
                List<DateTime> time = new List<DateTime>();
                int csvLength = System.IO.File.ReadAllLines("chirp_cli_db.csv").Length;

                foreach (var i in Enumerable.Range(0, csvLength))
                {
                    var line = reader.ReadLine();
                    if (i == 0)
                        continue;

                    if (line == null)
                        continue;

                    var values = Regex.Split(line, "[,]{1}(?=(?:[^\\\"]*\\\"[^\\\"]*\\\")*(?![^\\\"]*\\\"))");

                    authors.Add(values[0]);
                    cheeps.Add(values[1]);
                    time.Add(DateTimeOffset.FromUnixTimeSeconds(long.Parse(values[2])).AddHours(2).DateTime);
                }

                foreach (var i in Enumerable.Range(0, csvLength - 1))
                {
                    Console.WriteLine(authors[i] + " @ " + time[i].ToString("MM/dd/yyyy HH:mm:ss") + ": " + cheeps[i]);
                }
            }
            */
        }
    }
