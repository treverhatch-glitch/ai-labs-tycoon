using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough148PresentationSpecTests
    {
        [Test]
        public void Manifest_148()
        {
            Assert.AreEqual(148, CampusIndexLaneManifestTailRefreshThrough148PresentationSpec.RefreshThroughBatch);
        }
    }
}