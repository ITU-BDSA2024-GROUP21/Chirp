using System.IO;
using System.Text.RegularExpressions;
using Chirp.CLI;
using System.Globalization;
using CsvHelper;
using System.Collections.Generic;
using SimpleDB;
using CommandLine;

class Program
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
        var database = new CSVDatabase();
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(o =>
            {
                if (o.Cheep)
                {
                    UserInterface.startCheep();
                    var input = Console.ReadLine();
                    if (input == null)
                        input = "";
                    var cheep = new SimpleDB.Cheep(Environment.UserName, input, DateTimeOffset.Now.ToUnixTimeSeconds());
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
    