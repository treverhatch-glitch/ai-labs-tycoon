using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

[Collection("GoldenRevertBatch12")]
public class GoldenRevertBatchThirtyTwoTests
{
    static void Revert(int schema) => GoldenRevertTestRunner.Run("golden-revert-batch32-run.sh", schema);

    [Fact]
    public void Schema_195_reverts_to_schema_194_digest() => Revert(195);

    [Fact]
    public void Schema_196_reverts_to_schema_195_digest() => Revert(196);

    [Fact]
    public void Schema_197_reverts_to_schema_196_digest() => Revert(197);
}