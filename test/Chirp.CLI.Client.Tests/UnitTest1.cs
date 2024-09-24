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

    }
}

public class InputOutputTest
{   
    
    [Fact]
    public void TestOutputWhenIncorrect()
    {
        var sw = new StringWriter();
        Console.SetOut(sw);
        string[] args = { "Invalid" };

        Program.Main(args);
        
        var output = sw.ToString().Trim();
        Assert.Equal("Write a proper command (--c, --cheep, --r, --read)", output);
        sw.Close();
    }
    
}


