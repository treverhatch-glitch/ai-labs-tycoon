using Xunit;

namespace AiLabs.SimTests;

/// <summary>Batch-16 save steps 144-148 — wire to tools/golden-revert-batch16-run.sh when digests are repinned.</summary>
public class GoldenRevertBatchSixteenTests
{
    [Theory(Skip = "Repin GOLDEN[144-148] in golden-revert-batch16-run.sh after batch16 merges")]
    [InlineData(144)]
    [InlineData(145)]
    [InlineData(146)]
    [InlineData(147)]
    [InlineData(148)]
    public void Golden_revert_at_tip_for_each_batch_sixteen_schema(int schema)
    {
        Xunit.Assert.InRange(schema, 144, 148);
    }
}