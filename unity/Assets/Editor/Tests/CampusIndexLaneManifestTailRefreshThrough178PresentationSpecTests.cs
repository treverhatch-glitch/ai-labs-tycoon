using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough178PresentationSpecTests
    {
        [Test]
        public void Manifest_178()
        {
            Assert.AreEqual(
                178,
                CampusIndexLaneManifestTailRefreshThrough178PresentationSpec.RefreshThroughBatch);
        }
    }
}