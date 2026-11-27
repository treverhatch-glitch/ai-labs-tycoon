using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough208PresentationSpecTests
    {
        [Test]
        public void Refresh_through_advances_from_205()
        {
            Assert.Greater(
                CampusPresentationTailRollupRefreshThrough208PresentationSpec.RefreshThroughBatch,
                CampusPresentationTailRollupRefreshThrough205PresentationSpec.RefreshThroughBatch);
        }
    }
}