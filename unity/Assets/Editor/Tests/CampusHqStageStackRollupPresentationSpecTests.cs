using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusHqStageStackRollupPresentationSpecTests
    {
        [Test]
        public void Band_count_matches_stack_spec()
        {
            Assert.AreEqual(
                CampusHqStageStackPresentationSpec.HqPresentationBandCount,
                CampusHqStageStackRollupPresentationSpec.HqBandCount);
        }

        [Test]
        public void Build_index_layers()
        {
            Assert.AreEqual(2, CampusHqStageStackRollupPresentationSpec.RoomBuildIndexLayers);
        }

        [Test]
        public void Lot_viewport_matches_stack_spec()
        {
            Assert.AreEqual(
                CampusHqStageStackPresentationSpec.LotViewportWidthFraction,
                CampusHqStageStackRollupPresentationSpec.LotViewportWidthFraction);
        }
    }
}