namespace Chirp.CLI.Client.Tests.End2EndTests;
using Chirp;
using System.Diagnostics;
using System;
using SimpleDB;


public class End2EndTests
{
	[Fact]
	public void testOutput() {
	string output = "";

    using (var process = new Process())
    {
        process.StartInfo.FileName = "dotnet";
        process.StartInfo.Arguments = "run --r";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.WorkingDirectory = "../../../../../src/Chirp.CLI";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardInput = true;
        process.Start();
        // Synchronously read the standard output of the spawned process.
        StreamReader reader = process.StandardOutput;
        output = reader.ReadToEnd();
        process.WaitForExit();
    }
	string cheeps = output.Split("\n")[0] + "\n" + output.Split("\n")[1] + "\n" + output.Split("\n")[2];
	var expectedOutput = "ropf @ 08-01-2023 12:09:20: Hello, BDSA students!\r\n" + 
                         "adho @ 08-02-2023 12:19:38: Welcome to the course!\r\n" +
                         "adho @ 08-02-2023 12:37:38: I hope you had a good summer.\r";
    Assert.Equal(expectedOutput, cheeps);




}
    


        
}


