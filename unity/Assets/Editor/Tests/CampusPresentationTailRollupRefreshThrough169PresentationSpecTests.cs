using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough169PresentationSpecTests
    {
        [Test]
        public void Refresh_169()
        {
            Assert.AreEqual(
                169,
                CampusPresentationTailRollupRefreshThrough169PresentationSpec.RefreshThroughBatch);
        }
    }
}