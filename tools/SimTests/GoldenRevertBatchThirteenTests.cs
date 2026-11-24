using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Batch-13 save steps 130-134 recover the previous golden at tip while new config blocks remain loaded.</summary>
[Collection("GoldenRevertBatch12")]
public class GoldenRevertBatchThirteenTests
{
    static void Revert(int schema) => GoldenRevertTestRunner.Run("golden-revert-batch13-run.sh", schema);


    [Theory]
    [InlineData(130)]
    [InlineData(131)]
    [InlineData(132)]
    [InlineData(133)]
    [InlineData(134)]
    public void Golden_revert_at_tip_for_each_batch_thirteen_schema(int schema) => Revert(schema);
}