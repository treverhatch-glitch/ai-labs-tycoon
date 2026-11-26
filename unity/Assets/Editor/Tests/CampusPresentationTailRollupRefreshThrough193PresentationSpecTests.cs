using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough193PresentationSpecTests
    {
        [Test]
        public void Refresh_through_advances_from_190()
        {
            Assert.Greater(
                CampusPresentationTailRollupRefreshThrough193PresentationSpec.RefreshThroughBatch,
                CampusPresentationTailRollupRefreshThrough190PresentationSpec.RefreshThroughBatch);
        }
    }
}