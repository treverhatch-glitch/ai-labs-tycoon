using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshPresentationSpecTests
    {
        [Test]
        public void Refresh_through_104()
        {
            Assert.AreEqual(104, CampusIndexLaneManifestTailRefreshPresentationSpec.RefreshThroughBatch);
        }

        [Test]
        public void Prior_tail_ends_78()
        {
            Assert.AreEqual(78, CampusIndexLaneManifestTailRefreshPresentationSpec.PriorTailBatchEnd);
        }

        [Test]
        public void Rollup_aligns_104()
        {
            Assert.AreEqual(104, CampusIndexLaneManifestTailRefreshPresentationSpec.RollupRefreshLastBatch);
        }
    }
}