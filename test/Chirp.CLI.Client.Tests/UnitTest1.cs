using System.Diagnostics;
using Chirp;
using System;
using System.IO;

namespace Chirp.CLI.Client.Tests;

public class timeConverterTests
{
    timeConverter timeConverter = new timeConverter();
    [Fact]
    public void TestDateTimeToUnixTime()
    {
        var dateAndTime = new DateTimeOffset(2008, 5, 1, 8, 6, 32,
            new TimeSpan(1, 0, 0));
        long convertedDateAndTime = timeConverter.ConvertToUnixTime(dateAndTime);
        
        Assert.Equal((long)1209625592, convertedDateAndTime);
	Console.Clear();
    }
	

}

public class InputOutputTest
{   
   
    [Fact]
    public void TestOutputWhenIncorrect()
    {
		
        string output = "";
        using (var process = new Process())
        {
            process.StartInfo.FileName = "/usr/local/share/dotnet/dotnet";
            process.StartInfo.Arguments = "run Invalid";
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
        string fstCheep = output.Split("\n")[0];
        // Assert
        Assert.Equal("Write a proper command (--c, --cheep, --r, --read)", fstCheep);
	
		
    }

	[Fact]
	public void TestOutputWhenCheep(){
	 string output1 = "";
        using (var process1 = new Process())
        {
            process1.StartInfo.FileName = "/usr/local/share/dotnet/dotnet";
            process1.StartInfo.Arguments = "run --cheep";
            process1.StartInfo.UseShellExecute = false;
            process1.StartInfo.WorkingDirectory = "../../../../../src/Chirp.CLI";
            process1.StartInfo.RedirectStandardOutput = true;

            process1.StartInfo.RedirectStandardInput = true;
            process1.Start();

            // Synchronously read the standard output of the spawned process.
            StreamReader reader = process1.StandardOutput;
            output1 = reader.ReadToEnd();
            process1.WaitForExit();
        }
        string fstCheep1 = output1.Split("\n")[0];
        // Assert
        Assert.Equal("Welcome to Chirp! Write your cheep:", fstCheep1);
}
	

}

