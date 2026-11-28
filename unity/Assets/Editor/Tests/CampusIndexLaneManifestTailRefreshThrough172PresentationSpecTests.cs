using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough172PresentationSpecTests
    {
        [Test]
        public void Manifest_172()
        {
            Assert.AreEqual(
                172,
                CampusIndexLaneManifestTailRefreshThrough172PresentationSpec.RefreshThroughBatch);
        }
    }
}