using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

[Collection("GoldenRevertBatch12")]
public class GoldenRevertBatchTwentyTests
{
    static void Revert(int schema) => GoldenRevertTestRunner.Run("golden-revert-batch20-run.sh", schema);


    [Fact]
    public void Schema_158_reverts_to_schema_157_digest() => Revert(158);

    [Fact]
    public void Schema_159_reverts_to_schema_158_digest() => Revert(159);

    [Fact]
    public void Schema_160_reverts_to_schema_159_digest() => Revert(160);
}