using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough166PresentationSpecTests
    {
        [Test]
        public void Manifest_166()
        {
            Assert.AreEqual(
                166,
                CampusIndexLaneManifestTailRefreshThrough166PresentationSpec.RefreshThroughBatch);
        }
    }
}