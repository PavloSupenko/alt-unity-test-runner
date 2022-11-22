using Shared.Arguments;
using TestsHtmlReportCreator.Arguments;


namespace TestsHtmlReportCreator;

public static class Program
{
    private static ArgumentsReader<Argument> argumentsReader;

    
    private static async Task Main(string[] args)
    {
        argumentsReader = new ArgumentsReader<Argument>(args, ArgumentValues.Keys,
            ArgumentValues.Defaults, ArgumentValues.Descriptions);

        if (TryShowHelp(argumentsReader))
            return;

        ReportBuilder reportBuilder = new ReportBuilder(
            localAndroid: argumentsReader[Argument.LocalAndroidPath],
            localIos: argumentsReader[Argument.LocalIosPath],
            remoteAndroid: argumentsReader[Argument.RemoteAndroidPath],
            remoteIos: argumentsReader[Argument.RemoteIosPath],
            path: argumentsReader[Argument.ReportPath]
        );
        
        reportBuilder.Build();
    }

    private static bool TryShowHelp(ArgumentsReader<Argument> argumentsReader)
    {
        if (!argumentsReader.IsTrue(Argument.Help)) 
            return false;

        var showDefaults = true;
        ArgumentsReader<Argument>.ShowHelp(argumentsReader, "==== Parameters: ====", showDefaults);
        Console.WriteLine();

        return true;
    }
}