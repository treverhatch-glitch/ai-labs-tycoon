using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough181PresentationSpecTests
    {
        [Test]
        public void Manifest_181()
        {
            Assert.AreEqual(181, CampusIndexLaneManifestTailRefreshThrough181PresentationSpec.RefreshThroughBatch);
        }
    }
}