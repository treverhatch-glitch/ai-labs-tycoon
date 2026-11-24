using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Batch-18 save steps 154-155 recover the previous golden at tip while newer config blocks remain loaded.</summary>
[Collection("GoldenRevertBatch12")]
public class GoldenRevertBatchEighteenTests
{
    static void Revert(int schema) => GoldenRevertTestRunner.Run("golden-revert-batch18-run.sh", schema);


    [Fact]
    public void Schema_154_reverts_to_schema_153_digest() => Revert(154);

    [Fact]
    public void Schema_155_reverts_to_schema_154_digest() => Revert(155);
}