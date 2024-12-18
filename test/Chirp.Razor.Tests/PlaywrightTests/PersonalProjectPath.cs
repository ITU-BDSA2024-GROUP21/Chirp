namespace Chirp.Razor.Tests.PlaywrightTests;

public class PersonalProjectPath
{
    // Personal absolute path to web app. Done this way to be able to include in gitignore

    private static string defaultPath = @"src\Chirp.Razor\Chirp.Web";

    private static string envVarName = "CHIRP_PROJECT_PATH";

    public static string path;

    public static string GetPath()
    {
        var customPath = Environment.GetEnvironmentVariable(envVarName);
        
        if (string.IsNullOrEmpty(customPath))
        {
            path = defaultPath;
        }
        else
        {
            path = customPath;
            Console.WriteLine($"Using custom path: {customPath}");
        }

        return path;
    }
}
