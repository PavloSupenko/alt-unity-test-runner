using System;
using System.Collections.Generic;
using System.Linq;


namespace TestsClient;

public class SelectedTestsRunner
{
    /// <summary>
    /// Split up test names from command line arguments and execute it in the right order.
    /// </summary>
    public static void RunTestFromCommandLine()
    {
        var arguments = Environment.GetCommandLineArgs().ToList();

        var testSwitchIndex = arguments.IndexOf("-tests");

        var nextSwitchIndex = arguments.IndexOf(
            arguments.FirstOrDefault(arg => arguments.IndexOf(arg) > testSwitchIndex && arg.StartsWith("-"))
            ?? arguments.Last());

        var testNames = arguments
            .Where(arg =>
            {
                var index = arguments.IndexOf(arg);
                return index > testSwitchIndex && index < nextSwitchIndex && !arg.StartsWith("-");
            })
            .ToArray();

        var testInvocationCommands = testNames.Select(name => new[] { "-tests", name }).ToList();

        foreach (var command in testInvocationCommands)
        {
            Console.WriteLine($"Executing prepared: {string.Join(" ", command)}");
        }

        foreach (var command in testInvocationCommands)
        {
            Console.WriteLine($"Executing: {string.Join(" ", command)}");
            RunForSelectedArguments(command);
        }
    }

    private static void RunForSelectedArguments(string[] arguments)
    {
        var testAssemblyRunner =
            new NUnit.Framework.Api.NUnitTestAssemblyRunner(new NUnit.Framework.Api.DefaultTestAssemblyBuilder());

        NUnit.Framework.Internal.TestSuite testSuite = null;
        System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();

        System.Reflection.Assembly assembly = assemblies.FirstOrDefault(assemblyName =>
            assemblyName.GetName().Name.Equals("Assembly-CSharp-Editor"));

        testAssemblyRunner.Load(assembly, new Dictionary<string, object>());
        //var result = testAssemblyRunner.Run(listener, filter);
    }
}