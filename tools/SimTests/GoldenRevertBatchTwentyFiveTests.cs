using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

[Collection("GoldenRevertBatch12")]
public class GoldenRevertBatchTwentyFiveTests
{
    static void Revert(int schema) => GoldenRevertTestRunner.Run("golden-revert-batch25-run.sh", schema);

    [Fact]
    public void Schema_174_reverts_to_schema_173_digest() => Revert(174);

    [Fact]
    public void Schema_175_reverts_to_schema_174_digest() => Revert(175);

    [Fact]
    public void Schema_176_reverts_to_schema_175_digest() => Revert(176);

    [Fact]
    public void Schema_177_reverts_to_schema_176_digest() => Revert(177);
}