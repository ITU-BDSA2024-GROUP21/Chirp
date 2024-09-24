using System.Collections.ObjectModel;

namespace Chirp.CLI.Client.Tests;
using SimpleDB;
using System.Diagnostics;

/*
public class E2ETests
{
    //string pathToDataBase =  "../../../../../data/chirp_cli_db.csv";
    
    [Fact]
    public void CheepFunctionalityTests()
    {
        var dataBase = CSVDatabase.Instance;
        dataBase.csvPath = "../../../../../data/chirp_cli_db.csv";

        var simulatedInput = new StringReader("Hello!!");
        Console.SetIn(simulatedInput);
        
        var sw1 = new StringWriter(); 
        Console.SetOut(sw1); 
        
        string[] args = {"../../../../../data/chirp_cli_db.csv", "--cheep"};
          
        Program.Main(args);
          
        var output = sw1.ToString().Trim();
        Assert.Contains("Welcome to Chirp! Write your cheep:", output);

        var cheeps = dataBase.Read().ToList();
        var lastcheep = cheeps.Last().Message;


        Assert.Equal("Hello!!", lastcheep);

        sw.Close();
    }
}
*/
