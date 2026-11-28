using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough121PresentationSpecTests
    {
        [Test]
        public void Refresh_through_121()
        {
            Assert.AreEqual(121, CampusIndexLaneManifestTailRefreshThrough121PresentationSpec.RefreshThroughBatch);
        }

        [Test]
        public void Prior_118()
        {
            Assert.AreEqual(118, CampusIndexLaneManifestTailRefreshThrough121PresentationSpec.PriorRefreshThroughBatch);
        }

        [Test]
        public void Rollup_and_status_121()
        {
            Assert.AreEqual(121, CampusIndexLaneManifestTailRefreshThrough121PresentationSpec.RollupRefreshLastBatch);
            Assert.AreEqual(121, CampusIndexLaneManifestTailRefreshThrough121PresentationSpec.StatusSyncLastBatch);
        }
    }
}