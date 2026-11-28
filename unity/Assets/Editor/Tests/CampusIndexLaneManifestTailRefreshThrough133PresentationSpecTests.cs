using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough133PresentationSpecTests
    {
        [Test]
        public void Refresh_through_133()
        {
            Assert.AreEqual(133, CampusIndexLaneManifestTailRefreshThrough133PresentationSpec.RefreshThroughBatch);
        }

        [Test]
        public void Rollup_and_status_133()
        {
            Assert.AreEqual(133, CampusIndexLaneManifestTailRefreshThrough133PresentationSpec.RollupRefreshLastBatch);
            Assert.AreEqual(133, CampusIndexLaneManifestTailRefreshThrough133PresentationSpec.StatusSyncLastBatch);
        }
    }
}