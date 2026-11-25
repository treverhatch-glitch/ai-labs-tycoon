using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Batch-16 save steps 144-148 recover the previous golden at tip while newer config blocks remain loaded.</summary>
[Collection("GoldenRevertBatch12")]
public class GoldenRevertBatchSixteenTests
{
    static void Revert(int schema) => GoldenRevertTestRunner.Run("golden-revert-batch16-run.sh", schema);


    [Theory]
    [InlineData(144)]
    [InlineData(145)]
    [InlineData(146)]
    [InlineData(147)]
    [InlineData(148)]
    public void Golden_revert_at_tip_for_each_batch_sixteen_schema(int schema) => Revert(schema);
}