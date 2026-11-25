using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough178PresentationSpecTests
    {
        [Test]
        public void Refresh_178()
        {
            Assert.AreEqual(
                178,
                CampusPresentationTailRollupRefreshThrough178PresentationSpec.RefreshThroughBatch);
        }
    }
}