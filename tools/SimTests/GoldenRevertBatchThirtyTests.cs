using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

[Collection("GoldenRevertBatch12")]
public class GoldenRevertBatchThirtyTests
{
    static void Revert(int schema) => GoldenRevertTestRunner.Run("golden-revert-batch30-run.sh", schema);

    [Fact]
    public void Schema_190_reverts_to_schema_189_digest() => Revert(190);

    [Fact]
    public void Schema_191_reverts_to_schema_190_digest() => Revert(191);

    [Fact]
    public void Schema_192_reverts_to_schema_191_digest() => Revert(192);
}