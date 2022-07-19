using Newtonsoft.Json;
namespace TestsTreeParser.Tree;

public class TestsTreeNode
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Test;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<TestsTreeNode> SubTests;
}
