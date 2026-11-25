using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough142PresentationSpecTests
    {
        [Test]
        public void Manifest_142()
        {
            Assert.AreEqual(142, CampusIndexLaneManifestTailRefreshThrough142PresentationSpec.RefreshThroughBatch);
        }
    }
}