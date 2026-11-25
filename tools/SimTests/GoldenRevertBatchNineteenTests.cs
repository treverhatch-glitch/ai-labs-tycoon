using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

[Collection("GoldenRevertBatch12")]
public class GoldenRevertBatchNineteenTests
{
    static void Revert(int schema) => GoldenRevertTestRunner.Run("golden-revert-batch19-run.sh", schema);


    [Fact]
    public void Schema_156_reverts_to_schema_155_digest() => Revert(156);

    [Fact]
    public void Schema_157_reverts_to_schema_156_digest() => Revert(157);
}