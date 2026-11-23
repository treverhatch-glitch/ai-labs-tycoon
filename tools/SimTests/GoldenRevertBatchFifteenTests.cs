using System.IO;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class GoldenRevertBatchFifteenTests
{
    [Theory]
    [InlineData(139)]
    [InlineData(140)]
    [InlineData(141)]
    [InlineData(142)]
    [InlineData(143)]
    public void Revert_script_exists(int schema)
    {
        string script = Path.Combine(TestData.RepoRoot(), "tools", "golden-revert-batch15-run.sh");
        Assert.True(File.Exists(script + " " + schema) || File.Exists(script));
    }
}