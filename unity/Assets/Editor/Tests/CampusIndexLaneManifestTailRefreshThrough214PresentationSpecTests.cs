using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough214PresentationSpecTests
    {
        [Test]
        public void Refresh_through_advances_from_211()
        {
            Assert.Greater(
                CampusIndexLaneManifestTailRefreshThrough214PresentationSpec.RefreshThroughBatch,
                CampusIndexLaneManifestTailRefreshThrough211PresentationSpec.RefreshThroughBatch);
        }
    }
}