using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough112PresentationSpecTests
    {
        [Test]
        public void Refresh_through_112()
        {
            Assert.AreEqual(112, CampusIndexLaneManifestTailRefreshThrough112PresentationSpec.RefreshThroughBatch);
        }

        [Test]
        public void Prior_refresh_104()
        {
            Assert.AreEqual(104, CampusIndexLaneManifestTailRefreshThrough112PresentationSpec.PriorRefreshThroughBatch);
        }

        [Test]
        public void Rollup_and_status_align()
        {
            Assert.AreEqual(112, CampusIndexLaneManifestTailRefreshThrough112PresentationSpec.RollupRefreshLastBatch);
            Assert.AreEqual(109, CampusIndexLaneManifestTailRefreshThrough112PresentationSpec.StatusSyncLastBatch);
        }
    }
}