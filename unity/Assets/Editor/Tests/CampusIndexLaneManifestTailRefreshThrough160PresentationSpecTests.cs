using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough160PresentationSpecTests
    {
        [Test]
        public void Manifest_160()
        {
            Assert.AreEqual(160, CampusIndexLaneManifestTailRefreshThrough160PresentationSpec.RefreshThroughBatch);
        }
    }
}