using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough205PresentationSpecTests
    {
        [Test]
        public void Refresh_through_advances_from_202()
        {
            Assert.Greater(
                CampusPresentationTailRollupRefreshThrough205PresentationSpec.RefreshThroughBatch,
                CampusPresentationTailRollupRefreshThrough202PresentationSpec.RefreshThroughBatch);
        }
    }
}