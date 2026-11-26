using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailRefreshThrough205PresentationSpecTests
    {
        [Test]
        public void Refresh_through_advances_from_202()
        {
            Assert.Greater(
                CampusIndexLaneManifestTailRefreshThrough205PresentationSpec.RefreshThroughBatch,
                CampusIndexLaneManifestTailRefreshThrough202PresentationSpec.RefreshThroughBatch);
        }
    }
}