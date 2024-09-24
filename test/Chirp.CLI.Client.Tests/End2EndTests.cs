namespace Chirp.CLI.Client.Tests.End2EndTests;
using Chirp;
using System.Diagnostics;
using System;
using SimpleDB;

/*
public class End2EndTests
{
    [Fact]
    public void testOutput()
    {
		var database = new CSVDatabase();
        string csvPath = "../../../../../data/chirp_cli_db.csv";
        database.csvPath = csvPath;
        
        var cheeps = database.Read().Take(3).ToList();
        var expectedOutput = "ropf @ 08-01-2023 12:09:20: Hello, BDSA students!\r\n" + 
                             "adho @ 08-02-2023 12:19:38: Welcome to the course!\r\n" +
                             "adho @ 08-02-2023 12:37:38: I hope you had a good summer.\r\n";
        using (var sw = new StringWriter())
        {
            Console.SetOut(sw);
            UserInterface.PrintCheeps(cheeps);
            var output = sw.ToString();
            Assert.Equal(expectedOutput, output);
        }

    }
        
}

*/