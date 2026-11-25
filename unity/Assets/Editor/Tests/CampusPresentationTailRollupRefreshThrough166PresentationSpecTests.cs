using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough166PresentationSpecTests
    {
        [Test]
        public void Refresh_166()
        {
            Assert.AreEqual(
                166,
                CampusPresentationTailRollupRefreshThrough166PresentationSpec.RefreshThroughBatch);
        }
    }
}