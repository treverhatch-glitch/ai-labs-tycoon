using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Batch-15 save steps 139-143 recover the previous golden at tip while new config blocks remain loaded.</summary>
[Collection("GoldenRevertBatch12")]
public class GoldenRevertBatchFifteenTests
{
    static void Revert(int schema) => GoldenRevertTestRunner.Run("golden-revert-batch15-run.sh", schema);


    [Theory]
    [InlineData(139)]
    [InlineData(140)]
    [InlineData(141)]
    [InlineData(142)]
    [InlineData(143)]
    public void Golden_revert_at_tip_for_each_batch_fifteen_schema(int schema) => Revert(schema);
}