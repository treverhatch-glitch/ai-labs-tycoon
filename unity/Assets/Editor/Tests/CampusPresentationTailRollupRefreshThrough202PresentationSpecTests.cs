using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough202PresentationSpecTests
    {
        [Test]
        public void Refresh_through_advances_from_199()
        {
            Assert.Greater(
                CampusPresentationTailRollupRefreshThrough202PresentationSpec.RefreshThroughBatch,
                CampusPresentationTailRollupRefreshThrough199PresentationSpec.RefreshThroughBatch);
        }
    }
}