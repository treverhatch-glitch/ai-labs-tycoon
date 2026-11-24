using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough145PresentationSpecTests
    {
        [Test]
        public void Manifest_145()
        {
            Assert.AreEqual(145, CampusIndexLaneManifestTailRefreshThrough145PresentationSpec.RefreshThroughBatch);
        }
    }
}