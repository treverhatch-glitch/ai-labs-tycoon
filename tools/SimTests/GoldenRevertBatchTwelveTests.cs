using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

[CollectionDefinition("GoldenRevertBatch12", DisableParallelization = true)]
public sealed class GoldenRevertBatchTwelveCollection;

/// <summary>Batch-12 save steps 125-129 recover the previous golden at tip while new config blocks remain loaded.</summary>
[Collection("GoldenRevertBatch12")]
public class GoldenRevertBatchTwelveTests
{
    static void Revert(int schema) => GoldenRevertTestRunner.Run("golden-revert-batch12-run.sh", schema);


    [Theory]
    [InlineData(125)]
    [InlineData(126)]
    [InlineData(127)]
    [InlineData(128)]
    [InlineData(129)]
    public void Golden_revert_at_tip_for_each_batch_twelve_schema(int schema) => Revert(schema);
}