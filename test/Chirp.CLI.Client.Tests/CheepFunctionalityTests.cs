using System.Collections.ObjectModel;

namespace Chirp.CLI.Client.Tests;
using SimpleDB;
using System.Diagnostics;

public class E2ETests
{
    //string pathToDataBase =  "../../../../../data/chirp_cli_db.csv";
    
    [Fact]
    public void CheepFunctionalityTests()
    {
        var dataBase = new CSVDatabase();
        dataBase.csvPath = "../../../../../data/chirp_cli_db.csv";

        var simulatedInput = new StringReader("Hello!!");
        Console.SetIn(simulatedInput);
        
        var sw = new StringWriter(); 
        Console.SetOut(sw); 
        
        string[] args = {"../../../../../data/chirp_cli_db.csv", "--cheep"};
          
        Program.Main(args);
          
        var output = sw.ToString().Trim();
        Assert.Contains("Welcome to Chirp! Write your cheep:", output);

        var cheeps = dataBase.Read().ToList();
        var lastcheep = cheeps.Last().Message;


        Assert.Equal("Hello!!", lastcheep);
  }
}
