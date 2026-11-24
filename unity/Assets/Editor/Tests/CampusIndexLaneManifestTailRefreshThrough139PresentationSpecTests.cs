using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough139PresentationSpecTests
    {
        [Test]
        public void Manifest_139()
        {
            Assert.AreEqual(139, CampusIndexLaneManifestTailRefreshThrough139PresentationSpec.RefreshThroughBatch);
            Assert.AreEqual(139, CampusIndexLaneManifestTailRefreshThrough139PresentationSpec.StatusSyncLastBatch);
        }
    }
}