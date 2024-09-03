namespace Chirp.CLI;
using System;  
public class cheep {
    static String input = "";
    private String csvFilePath = "chirp_cli_db.csv";
    public static void test(string[] args)
    {
        Console.WriteLine("make Cheep");
        input = Console.ReadLine();
        Console.WriteLine(input + " " + Environment.UserName + " " + DateTimeOffset.Now.ToUnixTimeSeconds());
        string path = @"chirp_cli_db.csv";
        using (StreamWriter sw = File.AppendText(path))
        {
            sw.WriteLine(Environment.UserName + ",\"" + input + "\"," + DateTimeOffset.Now.ToUnixTimeSeconds());
        }
    }
    
}