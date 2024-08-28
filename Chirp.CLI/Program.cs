using System.IO;

using (var reader = new StreamReader("chirp_cli_db.csv"))
{
    List<String> Authors = new List<String>();
    List<String> cheeps = new List<String>();
    List<long> time  = new List<long>();
    int csvLength = System.IO.File.ReadAllLines("chirp_cli_db.csv").Length;
    
    Console.WriteLine(csvLength);
    foreach (var i in Enumerable.Range(0, csvLength))
    {
        var line = reader.ReadLine();
        if (i == 0)
            continue;
        
        var values = line.Split(',');
        Authors.Add(values[0]);
        cheeps.Add(values[1]);
        time.Add(long.Parse(values[2]));
    }
    Console.WriteLine(Authors[0]);
    
}

