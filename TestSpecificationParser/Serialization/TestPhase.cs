namespace TestSpecificationParser;

public class TestPhase
{
    [YamlDotNet.Serialization.YamlMember(Alias="commands")]
    public List<string> Commands;
}