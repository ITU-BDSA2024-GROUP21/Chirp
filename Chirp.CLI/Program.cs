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
        [Option('c', "cheep", Required = true, HelpText = "Write your cheep.")]
        public bool Cheep { get; set; }
        
        [Option('r', "read", Required = true, HelpText = "Read all cheeps.")]
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
                    var input = Console.ReadLine();
                    var cheep = new SimpleDB.Cheep(Environment.UserName, input, DateTimeOffset.Now.ToUnixTimeSeconds());
                    database.Store(cheep);
                }
                if (o.Read)
                {
                    var records = database.Read();
                    UserInterface.PrintCheeps(records);
                }
                else
                {
                    Console.WriteLine("Write a proper command");
                }
            });
        /*
        var database = new CSVDatabase();
        if (args.Length > 0)
        {
            switch (args[0])
            {
                case "cheep":
                    Console.WriteLine("make Cheep");
                    var input = Console.ReadLine();
                    var cheep = new SimpleDB.Cheep(Environment.UserName, input, DateTimeOffset.Now.ToUnixTimeSeconds());
                    database.Store(cheep);
                    return;
                
                case "read":
                    var records = database.Read();
                    UserInterface.PrintCheeps(records);

                    return;
            }
        }
        */
    }
}
    