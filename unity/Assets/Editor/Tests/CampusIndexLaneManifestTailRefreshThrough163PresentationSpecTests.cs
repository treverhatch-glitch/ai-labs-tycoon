using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough163PresentationSpecTests
    {
        [Test]
        public void Manifest_163()
        {
            Assert.AreEqual(
                163,
                CampusIndexLaneManifestTailRefreshThrough163PresentationSpec.RefreshThroughBatch);
        }
    }
}