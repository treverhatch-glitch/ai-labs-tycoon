using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough169PresentationSpecTests
    {
        [Test]
        public void Manifest_169()
        {
            Assert.AreEqual(
                169,
                CampusIndexLaneManifestTailRefreshThrough169PresentationSpec.RefreshThroughBatch);
        }
    }
}