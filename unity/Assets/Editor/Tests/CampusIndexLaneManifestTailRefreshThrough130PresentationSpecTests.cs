using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough130PresentationSpecTests
    {
        [Test]
        public void Refresh_through_130()
        {
            Assert.AreEqual(130, CampusIndexLaneManifestTailRefreshThrough130PresentationSpec.RefreshThroughBatch);
        }

        [Test]
        public void Rollup_and_status_130()
        {
            Assert.AreEqual(130, CampusIndexLaneManifestTailRefreshThrough130PresentationSpec.RollupRefreshLastBatch);
            Assert.AreEqual(130, CampusIndexLaneManifestTailRefreshThrough130PresentationSpec.StatusSyncLastBatch);
        }
    }
}