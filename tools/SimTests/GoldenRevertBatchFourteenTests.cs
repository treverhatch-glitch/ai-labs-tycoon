using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Batch-13 save steps 135-138 recover the previous golden at tip while new config blocks remain loaded.</summary>
[Collection("GoldenRevertBatch12")]
public class GoldenRevertBatchFourteenTests
{
    static void Revert(int schema) => GoldenRevertTestRunner.Run("golden-revert-batch14-run.sh", schema);


    [Theory]
    [InlineData(135)]
    [InlineData(136)]
    [InlineData(137)]
    [InlineData(138)]
    public void Golden_revert_at_tip_for_each_batch_fourteen_schema(int schema) => Revert(schema);
}