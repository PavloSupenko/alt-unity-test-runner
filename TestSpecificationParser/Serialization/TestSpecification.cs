namespace TestSpecificationParser;

public class TestSpecification
{
    [YamlDotNet.Serialization.YamlMember(Alias="version")]
    public float Version;
    
    [YamlDotNet.Serialization.YamlMember(Alias="phases")]
    public Dictionary<string, TestPhase> Phases;
    
    [YamlDotNet.Serialization.YamlMember(Alias="artifacts")]
    public List<string> Artifacts;
}