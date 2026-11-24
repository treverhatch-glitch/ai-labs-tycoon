using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough127PresentationSpecTests
    {
        [Test]
        public void Refresh_through_127()
        {
            Assert.AreEqual(127, CampusIndexLaneManifestTailRefreshThrough127PresentationSpec.RefreshThroughBatch);
        }

        [Test]
        public void Rollup_and_status_127()
        {
            Assert.AreEqual(127, CampusIndexLaneManifestTailRefreshThrough127PresentationSpec.RollupRefreshLastBatch);
            Assert.AreEqual(127, CampusIndexLaneManifestTailRefreshThrough127PresentationSpec.StatusSyncLastBatch);
        }
    }
}