using System.Diagnostics;

namespace Chirp.Razor.Tests.PlaywrightTests;

    public static class ServerUtil
    {
        public static Task<Process> StartServer()
        {
            string projectPath = PersonalProjectPath.GetPath();

            var serverProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"run --project {projectPath}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            serverProcess.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    Console.WriteLine("Output: " + args.Data);
                }
            };

            serverProcess.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    Console.WriteLine("Error: " + args.Data);
                }
            };

            serverProcess.Start();
            serverProcess.BeginOutputReadLine();
            serverProcess.BeginErrorReadLine();

            // Wait for server to start up and become ready

            return Task.FromResult(serverProcess);
        }
    }