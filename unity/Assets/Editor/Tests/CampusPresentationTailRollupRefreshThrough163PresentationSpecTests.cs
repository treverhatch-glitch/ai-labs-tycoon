using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough163PresentationSpecTests
    {
        [Test]
        public void Refresh_163()
        {
            Assert.AreEqual(
                163,
                CampusPresentationTailRollupRefreshThrough163PresentationSpec.RefreshThroughBatch);
        }
    }
}