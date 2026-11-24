using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough118PresentationSpecTests
    {
        [Test]
        public void Refresh_through_118()
        {
            Assert.AreEqual(118, CampusIndexLaneManifestTailRefreshThrough118PresentationSpec.RefreshThroughBatch);
        }

        [Test]
        public void Prior_115()
        {
            Assert.AreEqual(115, CampusIndexLaneManifestTailRefreshThrough118PresentationSpec.PriorRefreshThroughBatch);
        }

        [Test]
        public void Rollup_and_status_118()
        {
            Assert.AreEqual(118, CampusIndexLaneManifestTailRefreshThrough118PresentationSpec.RollupRefreshLastBatch);
            Assert.AreEqual(118, CampusIndexLaneManifestTailRefreshThrough118PresentationSpec.StatusSyncLastBatch);
        }
    }
}