namespace Chirp.CLI.Client.Tests;
using SimpleDB;
using System.Diagnostics;

public class E2ETests
{
    [Fact]
    public void CheepFunctionalityTests()
    {
        var dataBase = new CSVDatabase();
        string CSVPath = "../../data/chirp_cli_db.csv";
        dataBase.csvPath = CSVPath;

        string testMessage = "Hello!!!";



        using (var process = new Process())
        {
            process.StartInfo.FileName = "/usr/bin/dotnet";
            process.StartInfo.Arguments = "../src/Chirp.CLI.Client/bin/Debug/net7.0/Chirp.CLI.dll --cheep Hello!!!";
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.WorkingDirectory = "../../../";
            process.StartInfo.RedirectStandardOutput = false;
            process.Start();
            process.WaitForExit();


        }

        string filePath = "/Users/stinewittendorff/Desktop/Chirp/data";
        string fileContent = File.ReadAllText(filePath);
        Assert.Contains(testMessage, fileContent);
        
       
    }
}