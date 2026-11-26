using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough190PresentationSpecTests
    {
        [Test]
        public void Refresh_through_advances_from_187()
        {
            Assert.Greater(
                CampusPresentationTailRollupRefreshThrough190PresentationSpec.RefreshThroughBatch,
                CampusPresentationTailRollupRefreshThrough187PresentationSpec.RefreshThroughBatch);
        }
    }
}