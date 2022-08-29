namespace TestsRunner.Arguments;

public class ArgumentsReader<TArgsEnum> where TArgsEnum : Enum
{
    private readonly Dictionary<TArgsEnum, string> @default;
    private readonly Dictionary<TArgsEnum, string> commandLineValues;
    private readonly Dictionary<TArgsEnum, string> descriptions;
    private readonly IReadOnlyDictionary<string, TArgsEnum> allCommandLineKeys;

    public ArgumentsReader(
        IEnumerable<string> commandLineArguments,
        IReadOnlyDictionary<string, TArgsEnum> commandLineKeys,
        Dictionary<TArgsEnum, string> @default,
        Dictionary<TArgsEnum, string> descriptions)
    {
        this.descriptions = descriptions;
        this.@default = @default;
        this.allCommandLineKeys = commandLineKeys;
        this.commandLineValues = ParseCommandLineArguments(commandLineArguments, commandLineKeys);
    }

    private Dictionary<TArgsEnum, string> ParseCommandLineArguments(
        IEnumerable<string> commandLineArguments,
        IReadOnlyDictionary<string, TArgsEnum> commandLineKeys)
    {
        var lineArguments = commandLineArguments.ToArray();
        var result = new Dictionary<TArgsEnum, string>();

        for (var i = 0; i < lineArguments.Length; i++)
        {
            var argument = lineArguments[i];

            if (commandLineKeys.ContainsKey(argument))
            {
                if (!result.ContainsKey(commandLineKeys[argument]))
                    result.Add(commandLineKeys[argument], lineArguments[i + 1]);

                i++;
            }
        }

        return result;
    }

    public string this[TArgsEnum argument]
    {
        get
        {
            if (commandLineValues.ContainsKey(argument))
                return commandLineValues[argument];

            if (@default.ContainsKey(argument))
                return @default[argument];

            return null;
        }
    }

    public (string switchName, string description) GetHelp(TArgsEnum argument)
    {
        var switchNames = allCommandLineKeys.Where(key => key.Value.Equals(argument)).Select(key => key.Key);
        var switchName = string.Join('/', switchNames);
        var description = descriptions.ContainsKey(argument)
            ? descriptions.FirstOrDefault(key => key.Key.Equals(argument)).Value
            : "*no description for this parameter*";

        return (switchName, description);
    }
}
