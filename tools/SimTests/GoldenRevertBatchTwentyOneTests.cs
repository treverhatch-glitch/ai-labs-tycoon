using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

[Collection("GoldenRevertBatch12")]
public class GoldenRevertBatchTwentyOneTests
{
    static void Revert(int schema) => GoldenRevertTestRunner.Run("golden-revert-batch21-run.sh", schema);


    [Fact]
    public void Schema_161_reverts_to_schema_160_digest() => Revert(161);

    [Fact]
    public void Schema_162_reverts_to_schema_161_digest() => Revert(162);

    [Fact]
    public void Schema_163_reverts_to_schema_162_digest() => Revert(163);
}