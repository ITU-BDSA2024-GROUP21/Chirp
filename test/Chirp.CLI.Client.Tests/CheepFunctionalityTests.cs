using System.Collections.ObjectModel;

namespace Chirp.CLI.Client.Tests;
using SimpleDB;
using System.Diagnostics;

public class E2ETests
{
    string pathToDataBase =  "../../data/chirp_cli_db.csv";

    [Fact]
   public void CheepFunctionalityTests()
   {
       string cheepMessage = "Hello!!!";
       var processStartInfo = new ProcessStartInfo
       {
           FileName = "dotnet",
           Arguments = "run --cheep ",
           RedirectStandardInput = true, 
           RedirectStandardOutput = true, 
           RedirectStandardError = true, 
           UseShellExecute = false
       };
       

       using (var process = new Process { StartInfo = processStartInfo })
       {
           process.Start();

           
           using (StreamWriter writer = new StreamWriter(pathToDataBase, append: true))
           {
               writer.WriteLine(cheepMessage);
               writer.Flush();
           }
           

           process.WaitForExit();
           System.Threading.Thread.Sleep(100);

           
           var lines = File.ReadAllLines(pathToDataBase).Last();
           Assert.EndsWith(lines, "Hello!!!"); 
       }
   }
}
