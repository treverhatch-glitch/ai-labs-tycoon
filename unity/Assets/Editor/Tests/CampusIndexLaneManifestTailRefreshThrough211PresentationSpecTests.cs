using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough211PresentationSpecTests
    {
        [Test]
        public void Refresh_through_advances_from_208()
        {
            Assert.Greater(
                CampusIndexLaneManifestTailRefreshThrough211PresentationSpec.RefreshThroughBatch,
                CampusIndexLaneManifestTailRefreshThrough208PresentationSpec.RefreshThroughBatch);
        }
    }
}