namespace Shared.Arguments;

public class ArgumentsReader<TArgsEnum> where TArgsEnum : Enum
{
    private readonly Dictionary<TArgsEnum, string> defaults;
    private readonly Dictionary<TArgsEnum, string> commandLineValues;
    private readonly Dictionary<TArgsEnum, string> descriptions;
    private readonly IReadOnlyDictionary<string, TArgsEnum> allCommandLineKeys;

    public ArgumentsReader(
        IEnumerable<string> commandLineArguments,
        IReadOnlyDictionary<string, TArgsEnum> commandLineKeys,
        Dictionary<TArgsEnum, string> defaults,
        Dictionary<TArgsEnum, string> descriptions)
    {
        this.defaults = defaults;
        this.descriptions = descriptions;
        this.allCommandLineKeys = commandLineKeys;
        this.commandLineValues = ParseCommandLineArguments(commandLineArguments, commandLineKeys);
    }

    public string this[TArgsEnum argument]
    {
        get
        {
            if (commandLineValues.ContainsKey(argument))
                return commandLineValues[argument];

            if (defaults.ContainsKey(argument))
                return defaults[argument];

            return null;
        }
    }
    
    public bool IsExist(TArgsEnum argument) => 
        this[argument] != null;
    
    public bool IsTrue(TArgsEnum argument) => 
        IsExist(argument) && this[argument].Equals("true");

    public (string switchName, string description) GetHelp(TArgsEnum argument)
    {
        var switchNames = allCommandLineKeys.Where(key => key.Value.Equals(argument)).Select(key => key.Key);
        var switchName = string.Join('/', switchNames);
        var description = descriptions.ContainsKey(argument)
            ? descriptions.FirstOrDefault(key => key.Key.Equals(argument)).Value
            : "*no description for this parameter*";

        return (switchName, description);
    }
    
    public static void ShowHelp(ArgumentsReader<TArgsEnum> argumentsReader, string header, bool showDefaults)
    {
        Console.WriteLine(header);
        foreach (TArgsEnum argumentValue in Enum.GetValues(typeof(TArgsEnum)))
        {
            var argumentHelp = argumentsReader.GetHelp(argumentValue);
            Console.WriteLine($"    [{argumentHelp.switchName}]  —  {argumentHelp.description}");
            
            if (showDefaults)
                Console.WriteLine($"        default value:{argumentsReader[argumentValue]}");
        }
    }

    private Dictionary<TArgsEnum, string> ParseCommandLineArguments(IEnumerable<string> commandLineArguments, 
        IReadOnlyDictionary<string, TArgsEnum> commandLineKeys)
    {
        var lineArguments = commandLineArguments.ToArray();
        var result = new Dictionary<TArgsEnum, string>();

        for (var i = 0; i < lineArguments.Length; i++)
        {
            var argument = lineArguments[i];

            if (!commandLineKeys.ContainsKey(argument)) 
                continue;

            if (result.ContainsKey(commandLineKeys[argument])) 
                continue;

            if (lineArguments.Length <= i + 1 || lineArguments[i + 1].StartsWith("-"))
            {
                result.Add(commandLineKeys[argument], "true");
            }
            else
            {
                result.Add(commandLineKeys[argument], lineArguments[i + 1]);
                i++;
            }
        }

        return result;
    }
}
