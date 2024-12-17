namespace Chirp.Razor.Tests.PlaywrightTests;

public class PersonalProjectPath
{
    // Personal absolute path to web app. Done this way to be able to include in gitignore

    public static string path = "../../../src/Chirp.Razor/Chirp.Web";

    public static string getPath()
    {
        return path;
    }
}
