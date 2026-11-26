using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough190PresentationSpecTests
    {
        [Test]
        public void Manifest_190()
        {
            Assert.AreEqual(190, CampusIndexLaneManifestTailRefreshThrough190PresentationSpec.RefreshThroughBatch);
        }
    }
}