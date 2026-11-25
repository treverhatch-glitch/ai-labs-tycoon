using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough175PresentationSpecTests
    {
        [Test]
        public void Refresh_175()
        {
            Assert.AreEqual(
                175,
                CampusPresentationTailRollupRefreshThrough175PresentationSpec.RefreshThroughBatch);
        }
    }
}