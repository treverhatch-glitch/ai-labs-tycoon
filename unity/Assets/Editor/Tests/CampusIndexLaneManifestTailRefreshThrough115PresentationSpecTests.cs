using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough115PresentationSpecTests
    {
        [Test]
        public void Refresh_through_115()
        {
            Assert.AreEqual(115, CampusIndexLaneManifestTailRefreshThrough115PresentationSpec.RefreshThroughBatch);
        }

        [Test]
        public void Prior_112()
        {
            Assert.AreEqual(112, CampusIndexLaneManifestTailRefreshThrough115PresentationSpec.PriorRefreshThroughBatch);
        }

        [Test]
        public void Rollup_and_status_115()
        {
            Assert.AreEqual(115, CampusIndexLaneManifestTailRefreshThrough115PresentationSpec.RollupRefreshLastBatch);
            Assert.AreEqual(115, CampusIndexLaneManifestTailRefreshThrough115PresentationSpec.StatusSyncLastBatch);
        }
    }
}