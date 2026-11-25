using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough175PresentationSpecTests
    {
        [Test]
        public void Manifest_175()
        {
            Assert.AreEqual(
                175,
                CampusIndexLaneManifestTailRefreshThrough175PresentationSpec.RefreshThroughBatch);
        }
    }
}