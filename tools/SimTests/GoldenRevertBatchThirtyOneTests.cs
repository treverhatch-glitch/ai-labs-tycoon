using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

[Collection("GoldenRevertBatch12")]
public class GoldenRevertBatchThirtyOneTests
{
    static void Revert(int schema) => GoldenRevertTestRunner.Run("golden-revert-batch31-run.sh", schema);

    [Fact]
    public void Schema_193_reverts_to_schema_192_digest() => Revert(193);

    [Fact]
    public void Schema_194_reverts_to_schema_193_digest() => Revert(194);

    [Fact]
    public void Schema_195_reverts_to_schema_194_digest() => Revert(195);
}