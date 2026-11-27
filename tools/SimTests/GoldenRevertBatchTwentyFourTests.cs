using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

[Collection("GoldenRevertBatch12")]
public class GoldenRevertBatchTwentyFourTests
{
    static void Revert(int schema) => GoldenRevertTestRunner.Run("golden-revert-batch24-run.sh", schema);

    [Fact]
    public void Schema_170_reverts_to_schema_169_digest() => Revert(170);

    [Fact]
    public void Schema_171_reverts_to_schema_170_digest() => Revert(171);

    [Fact]
    public void Schema_172_reverts_to_schema_171_digest() => Revert(172);
}