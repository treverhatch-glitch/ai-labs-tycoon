using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough193PresentationSpecTests
    {
        [Test]
        public void Manifest_batch_advances_from_190()
        {
            Assert.Greater(
                CampusIndexLaneManifestTailRefreshThrough193PresentationSpec.RefreshThroughBatch,
                CampusIndexLaneManifestTailRefreshThrough190PresentationSpec.RefreshThroughBatch);
        }
    }
}