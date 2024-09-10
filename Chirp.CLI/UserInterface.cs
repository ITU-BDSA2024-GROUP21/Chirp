namespace Chirp.CLI;
using SimpleDB;

public static class UserInterface
{
    public static void PrintCheeps(IEnumerable<SimpleDB.Cheep> cheeps)
    {
        foreach (var item in cheeps)
        {
            var timestamp = (int)item.Timestamp;
            var dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp);
            Console.WriteLine(item.Author + " @ " + dateTime.ToString("MM/dd/yyyy HH:mm:ss") + ": " +
                              item.Message);
        }
    }
}