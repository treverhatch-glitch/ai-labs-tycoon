using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough202PresentationSpecTests
    {
        [Test]
        public void Refresh_through_advances_from_199()
        {
            Assert.Greater(
                CampusIndexLaneManifestTailRefreshThrough202PresentationSpec.RefreshThroughBatch,
                CampusIndexLaneManifestTailRefreshThrough199PresentationSpec.RefreshThroughBatch);
        }
    }
}