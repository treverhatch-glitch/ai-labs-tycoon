using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough199PresentationSpecTests
    {
        [Test]
        public void Refresh_through_advances_from_196()
        {
            Assert.Greater(
                CampusPresentationTailRollupRefreshThrough199PresentationSpec.RefreshThroughBatch,
                CampusPresentationTailRollupRefreshThrough196PresentationSpec.RefreshThroughBatch);
        }
    }
}