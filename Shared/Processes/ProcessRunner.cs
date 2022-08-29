using System.Diagnostics;


namespace Shared.Processes;

public class ProcessRunner
{
    public void PrintProcessOutput(Process process, bool waitForProcessEnd = true)
    {
        var resultsStrings = GetProcessOutput(process, waitForProcessEnd).ToList();

        foreach (var line in resultsStrings)
            Console.WriteLine(line);

        Console.WriteLine();
    }

    public IEnumerable<string> GetProcessOutput(Process process, bool waitForProcessEnd = true)
    {
        var output = new List<string>();

        if (!waitForProcessEnd)
        {
            Thread.Sleep(TimeSpan.FromSeconds(10));
            var processOutput = process.StandardOutput.ReadToEnd();
            var processErrorOutput = process.StandardError.ReadToEnd();
            
            Console.WriteLine("Adding");
            output.Add(processOutput);
            output.Add(processErrorOutput);
            
            Console.WriteLine("Returning");
            return output;
        }

        while (!process.StandardOutput.EndOfStream)
        {
            var line = process.StandardOutput.ReadLine();

            if (line.Length > 0)
                output.Add(line);
        }

        process.WaitForExit();
        process.StandardError.ReadToEnd();
        
        return output;
    }

    public Process StartProcess(string processPath, string arguments, Dictionary<string, string>? variables = null,
        bool useDefaultStartInfo = false)
    {
        var process = new Process();
        var startInfo = useDefaultStartInfo
            ? new ProcessStartInfo
            {
                FileName = processPath,
                Arguments = arguments,
            }
            : new ProcessStartInfo
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Minimized,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                FileName = processPath,
                Arguments = arguments,
            };

        if (variables != null)
        {
            foreach (var variable in variables)
                startInfo.EnvironmentVariables[variable.Key] = variable.Value;
        }

        process.StartInfo = startInfo;
        process.Start();

        return process;
    }
}
