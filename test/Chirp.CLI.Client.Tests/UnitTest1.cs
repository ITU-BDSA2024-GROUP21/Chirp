using System.Diagnostics;
using Chirp;

namespace Chirp.CLI.Client.Tests;

public class timeConverterTests
{
    timeConverter timeConverter = new timeConverter(); 
    [Fact]
    public void TestDateTimeToUnixTime()
    {
        var dateAndTime = new DateTimeOffset(2008, 5, 1, 8, 6, 32,
            new TimeSpan(1, 0, 0));
        Console.WriteLine(dateAndTime);
        long convertedDateAndTime = timeConverter.ConvertToUnixTime(dateAndTime);
        
        Assert.Equal((long)1209625592, convertedDateAndTime);
    }
}