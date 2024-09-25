using System.Collections.ObjectModel;

namespace Chirp.CLI.Client.Tests;
using SimpleDB;
using System.Diagnostics;

public class E2ETests
{
    [Fact]
    public void CheepFunctionalityTests()
    {
        var dataBase = CSVDatabase.Instance;
        dataBase.csvPath = "../../../../../data/chirp_cli_db.csv";
        var cheepMessage = "Hello!!"; 
        string output = "";
        using (var process = new Process())
        {
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = "run --cheep";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = "../../../../../src/Chirp.CLI";
            process.StartInfo.RedirectStandardOutput = true;

            process.Start();
            
            StreamWriter sw = new StreamWriter(cheepMessage);
            //sw.WriteLine(cheepMessage);
            Console.SetOut(sw);

 
            process.WaitForExit();

            var cheeps = dataBase.Read().ToList();
            var lastcheep = cheeps.Last().Message;
            
            // Assert
            Assert.Equal("Hello!!", lastcheep); 
        }
    }
}

