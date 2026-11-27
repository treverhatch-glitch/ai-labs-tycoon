using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough211PresentationSpecTests
    {
        [Test]
        public void Refresh_through_advances_from_208()
        {
            Assert.Greater(
                CampusPresentationTailRollupRefreshThrough211PresentationSpec.RefreshThroughBatch,
                CampusPresentationTailRollupRefreshThrough208PresentationSpec.RefreshThroughBatch);
        }
    }
}