﻿namespace Chirp.Razor.Tests.PlaywrightTests;

public class PersonalProjectPath
{
    // Personal absolute path to web app. Done this way to be able to include in gitignore
    public static string path = @"C:\Users\krist\Chirp\src\Chirp.Razor\Chirp.Web\Chirp.Web.csproj";
    

    public static string getPath()
    {
        return path;
    }
}