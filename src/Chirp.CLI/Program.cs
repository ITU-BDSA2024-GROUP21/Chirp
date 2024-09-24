namespace Chirp;
using System.IO;
using System.Text.RegularExpressions;
using Chirp;
using System.Globalization;
using CsvHelper;
using System.Collections.Generic;
using SimpleDB;
using CommandLine;

public class timeConverter
{
    public long ConvertToUnixTime(DateTimeOffset time)
    {
        return time.ToUnixTimeSeconds();
    }
}

public class Program
{
    public class Options
    {
        [Option('c', "cheep", Required = false, HelpText = "Write your cheep.")]
        public bool Cheep { get; set; }

        [Option('r', "read", Required = false, HelpText = "Read all cheeps.")]
        public bool Read { get; set; }
    }

    public static void Main(string[] args)
    {
        var database = CSVDatabase.Instance;
        var csvPath = args.Length > 0 && args[0].EndsWith(".csv") ? args[0] : "../../data/chirp_cli_db.csv";
        database.csvPath = csvPath;
        var timeConverter = new timeConverter();
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(o =>
            {
                if (o.Cheep)
                {
                    Console.WriteLine("Current Directory: " + Directory.GetCurrentDirectory());
                    UserInterface.startCheep();
                    var input = Console.ReadLine();
                    if (input == null)
                        input = "";
                    var cheep = new SimpleDB.Cheep(Environment.UserName, input, timeConverter.ConvertToUnixTime(DateTimeOffset.Now));
                    database.Store(cheep);
                }
                else if (o.Read)
                {
                    var records = database.Read();
                    UserInterface.PrintCheeps(records);
                }
                else
                {
                    UserInterface.invalidCommand();
                }
            });
    }
    
}