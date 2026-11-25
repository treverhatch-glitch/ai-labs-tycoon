using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Batch-17 save steps 149-153 recover the previous golden at tip while newer config blocks remain loaded.</summary>
[Collection("GoldenRevertBatch12")]
public class GoldenRevertBatchSeventeenTests
{
    static void Revert(int schema) => GoldenRevertTestRunner.Run("golden-revert-batch17-run.sh", schema);


    [Theory]
    [InlineData(149)]
    [InlineData(150)]
    [InlineData(151)]
    [InlineData(152)]
    [InlineData(153)]
    public void Golden_revert_at_tip_for_each_batch_seventeen_schema(int schema) => Revert(schema);
}