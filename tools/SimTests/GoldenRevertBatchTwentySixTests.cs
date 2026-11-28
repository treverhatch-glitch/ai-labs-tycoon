using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

[Collection("GoldenRevertBatch12")]
public class GoldenRevertBatchTwentySixTests
{
    static void Revert(int schema) => GoldenRevertTestRunner.Run("golden-revert-batch26-run.sh", schema);

    [Fact]
    public void Schema_178_reverts_to_schema_177_digest() => Revert(178);

    [Fact]
    public void Schema_179_reverts_to_schema_178_digest() => Revert(179);

    [Fact]
    public void Schema_180_reverts_to_schema_179_digest() => Revert(180);
}