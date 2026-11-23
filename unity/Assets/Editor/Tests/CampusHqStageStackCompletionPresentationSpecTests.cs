using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusHqStageStackCompletionPresentationSpecTests
    {
        [Test]
        public void Indexed_bands_less_than_total()
        {
            Assert.Less(
                CampusHqStageStackCompletionPresentationSpec.IndexedHqBands,
                CampusHqStageStackCompletionPresentationSpec.TotalHqBands);
        }

        [Test]
        public void Total_matches_rollup()
        {
            Assert.AreEqual(
                CampusHqStageStackRollupPresentationSpec.HqBandCount,
                CampusHqStageStackCompletionPresentationSpec.TotalHqBands);
        }

        [Test]
        public void Build_layers_match_rollup()
        {
            Assert.AreEqual(
                CampusHqStageStackRollupPresentationSpec.RoomBuildIndexLayers,
                CampusHqStageStackCompletionPresentationSpec.RoomBuildIndexLayers);
        }
    }
}