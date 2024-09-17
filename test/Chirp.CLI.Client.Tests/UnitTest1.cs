using System.Diagnostics;
using Chirp.CLI;

namespace Chirp.CLI.Client.Tests;

public class UnitTest1
{
    Program CLIprogram = new Chirp.Program;
    [Fact]
    public void TestDateTimeToUnixTime()
    {
        var dateAndTime = new DateTimeOffset(2008, 5, 1, 8, 6, 32,
            new TimeSpan(1, 0, 0));
        Console.WriteLine(dateAndTime);
        long convertedDateAndTime = CLIprogram.convertDateTimeToUnixTime(dateAndTime);
        
        Assert.AreEqual((long)1209625592, convertedDateAndTime);


    }
}