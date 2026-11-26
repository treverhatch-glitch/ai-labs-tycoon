using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough184PresentationSpecTests
    {
        [Test]
        public void Manifest_184()
        {
            Assert.AreEqual(184, CampusIndexLaneManifestTailRefreshThrough184PresentationSpec.RefreshThroughBatch);
        }
    }
}