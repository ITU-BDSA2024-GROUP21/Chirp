﻿namespace Chirp.Razor;

public class Cheep
{
    public string Text { get; set; }
    public DateTime TimeStamp { get; set; }
    public int CheepId { get; set; }
    public Author Author { get; set; }
    public int AuthorId { get; set; }

}