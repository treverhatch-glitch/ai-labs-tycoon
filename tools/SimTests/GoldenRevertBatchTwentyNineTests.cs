using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

[Collection("GoldenRevertBatch12")]
public class GoldenRevertBatchTwentyNineTests
{
    static void Revert(int schema) => GoldenRevertTestRunner.Run("golden-revert-batch29-run.sh", schema);

    [Fact]
    public void Schema_186_reverts_to_schema_185_digest() => Revert(186);

    [Fact]
    public void Schema_187_reverts_to_schema_186_digest() => Revert(187);

    [Fact]
    public void Schema_188_reverts_to_schema_187_digest() => Revert(188);
}