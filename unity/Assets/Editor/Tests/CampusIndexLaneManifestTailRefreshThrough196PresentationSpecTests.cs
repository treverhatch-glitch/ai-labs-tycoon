using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough196PresentationSpecTests
    {
        [Test]
        public void Manifest_batch_advances_from_193()
        {
            Assert.Greater(
                CampusIndexLaneManifestTailRefreshThrough196PresentationSpec.RefreshThroughBatch,
                CampusIndexLaneManifestTailRefreshThrough193PresentationSpec.RefreshThroughBatch);
        }
    }
}