namespace TestsRunner;

public class ArgumentsReader<TArgsEnum> where TArgsEnum : Enum
{
    private readonly Dictionary<TArgsEnum, string> @default;
    private readonly Dictionary<TArgsEnum,string> commandLineValues;

    public ArgumentsReader(
        IEnumerable<string> commandLineArguments,
        IReadOnlyDictionary<string, TArgsEnum> commandLineKeys,
        Dictionary<TArgsEnum, string> @default)
    {
        this.@default = @default;
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
}
