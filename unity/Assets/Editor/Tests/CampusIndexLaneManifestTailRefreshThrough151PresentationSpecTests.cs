using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough151PresentationSpecTests
    {
        [Test]
        public void Manifest_151()
        {
            Assert.AreEqual(151, CampusIndexLaneManifestTailRefreshThrough151PresentationSpec.RefreshThroughBatch);
        }
    }
}