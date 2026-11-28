using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

[Collection("GoldenRevertBatch12")]
public class GoldenRevertBatchTwentyEightTests
{
    static void Revert(int schema) => GoldenRevertTestRunner.Run("golden-revert-batch28-run.sh", schema);

    [Fact]
    public void Schema_184_reverts_to_schema_183_digest() => Revert(184);

    [Fact]
    public void Schema_185_reverts_to_schema_184_digest() => Revert(185);

    [Fact]
    public void Schema_186_reverts_to_schema_185_digest() => Revert(186);
}