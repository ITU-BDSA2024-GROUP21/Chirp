using System.IO;
using System.Text.RegularExpressions;
using Chirp.CLI;
using System.Globalization;
using CsvHelper;
using System.Collections.Generic;
using SimpleDB;

class Program
{
    public static void Main(string[] args)
    {
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
    }
}
    