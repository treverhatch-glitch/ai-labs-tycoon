using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough196PresentationSpecTests
    {
        [Test]
        public void Refresh_through_advances_from_193()
        {
            Assert.Greater(
                CampusPresentationTailRollupRefreshThrough196PresentationSpec.RefreshThroughBatch,
                CampusPresentationTailRollupRefreshThrough193PresentationSpec.RefreshThroughBatch);
        }
    }
}