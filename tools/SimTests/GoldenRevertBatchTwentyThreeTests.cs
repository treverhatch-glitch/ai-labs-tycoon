using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

[Collection("GoldenRevertBatch12")]
public class GoldenRevertBatchTwentyThreeTests
{
    static void Revert(int schema) => GoldenRevertTestRunner.Run("golden-revert-batch23-run.sh", schema);


    [Fact]
    public void Schema_167_reverts_to_schema_166_digest() => Revert(167);

    [Fact]
    public void Schema_168_reverts_to_schema_167_digest() => Revert(168);

    [Fact]
    public void Schema_169_reverts_to_schema_168_digest() => Revert(169);
}