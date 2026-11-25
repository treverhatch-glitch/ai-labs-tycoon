using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough172PresentationSpecTests
    {
        [Test]
        public void Refresh_172()
        {
            Assert.AreEqual(
                172,
                CampusPresentationTailRollupRefreshThrough172PresentationSpec.RefreshThroughBatch);
        }
    }
}