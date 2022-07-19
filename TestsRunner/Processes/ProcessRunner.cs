using System.Diagnostics;


namespace TestsRunner.Processes;

public class ProcessRunner
{
    public void PrintProcessOutput(Process process)
    {
        var resultsStrings = GetProcessOutput(process).ToList();

        foreach (var line in resultsStrings)
            Console.WriteLine(line);

        Console.WriteLine();
    }

    public IEnumerable<string> GetProcessOutput(Process process)
    {
        var output = new List<string>();

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

    public Process StartProcess(string processPath, string arguments)
    {
        var process = new Process();

        var startInfo = new ProcessStartInfo
        {
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Minimized,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            FileName = processPath,
            Arguments = arguments
        };

        process.StartInfo = startInfo;
        process.Start();

        return process;
    }
}
