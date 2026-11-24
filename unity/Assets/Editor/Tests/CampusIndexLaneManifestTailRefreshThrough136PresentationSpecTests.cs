using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough136PresentationSpecTests
    {
        [Test]
        public void Refresh_through_136()
        {
            Assert.AreEqual(136, CampusIndexLaneManifestTailRefreshThrough136PresentationSpec.RefreshThroughBatch);
        }

        [Test]
        public void Rollup_and_status_136()
        {
            Assert.AreEqual(136, CampusIndexLaneManifestTailRefreshThrough136PresentationSpec.RollupRefreshLastBatch);
            Assert.AreEqual(136, CampusIndexLaneManifestTailRefreshThrough136PresentationSpec.StatusSyncLastBatch);
        }
    }
}