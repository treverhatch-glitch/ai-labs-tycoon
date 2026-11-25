using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

[Collection("GoldenRevertBatch12")]
public class GoldenRevertBatchTwentyTwoTests
{
    static void Revert(int schema) => GoldenRevertTestRunner.Run("golden-revert-batch22-run.sh", schema);


    [Fact]
    public void Schema_164_reverts_to_schema_163_digest() => Revert(164);

    [Fact]
    public void Schema_165_reverts_to_schema_164_digest() => Revert(165);

    [Fact]
    public void Schema_166_reverts_to_schema_165_digest() => Revert(166);
}