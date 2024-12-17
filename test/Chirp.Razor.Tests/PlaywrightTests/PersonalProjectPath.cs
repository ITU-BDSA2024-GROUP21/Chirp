namespace Chirp.Razor.Tests.PlaywrightTests;

public class PersonalProjectPath
{
    // Personal absolute path to web app. Done this way to be able to include in gitignore

    private static string defaultPath = @"src\Chirp.Razor\Chirp.Web";

    private static string envVarName = "CHIRP_PROJECT_PATH";

    public static string path = GetPath();

    public static string getPath()
    {
        string customPath = Environment.GetEnvironmentVariable(envVarName);
        return !string.IsNullOrEmpty(customPath) ? customPath : defaultPath;
    }
}
