using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough199PresentationSpecTests
    {
        [Test]
        public void Manifest_batch_advances_from_196()
        {
            Assert.Greater(
                CampusIndexLaneManifestTailRefreshThrough199PresentationSpec.RefreshThroughBatch,
                CampusIndexLaneManifestTailRefreshThrough196PresentationSpec.RefreshThroughBatch);
        }
    }
}