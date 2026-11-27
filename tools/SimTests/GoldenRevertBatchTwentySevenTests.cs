using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

[Collection("GoldenRevertBatch12")]
public class GoldenRevertBatchTwentySevenTests
{
    static void Revert(int schema) => GoldenRevertTestRunner.Run("golden-revert-batch27-run.sh", schema);

    [Fact]
    public void Schema_180_reverts_to_schema_179_digest() => Revert(180);

    [Fact]
    public void Schema_181_reverts_to_schema_180_digest() => Revert(181);

    [Fact]
    public void Schema_182_reverts_to_schema_181_digest() => Revert(182);
}