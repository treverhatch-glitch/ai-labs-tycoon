using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough160PresentationSpecTests
    {
        [Test]
        public void Refresh_160()
        {
            Assert.AreEqual(
                160,
                CampusPresentationTailRollupRefreshThrough160PresentationSpec.RefreshThroughBatch);
        }
    }
}