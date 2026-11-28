using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

[Collection("GoldenRevertBatch12")]
public class GoldenRevertBatchTwentySevenTests
{
    static void Revert(int schema) => GoldenRevertTestRunner.Run("golden-revert-batch27-run.sh", schema);

    [Fact]
    public void Schema_181_reverts_to_schema_180_digest() => Revert(181);

    [Fact]
    public void Schema_182_reverts_to_schema_181_digest() => Revert(182);

    [Fact]
    public void Schema_183_reverts_to_schema_182_digest() => Revert(183);
}