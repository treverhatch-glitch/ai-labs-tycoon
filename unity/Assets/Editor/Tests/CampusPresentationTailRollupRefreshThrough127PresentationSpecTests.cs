using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough127PresentationSpecTests
    {
        [Test]
        public void Refresh_through_127()
        {
            Assert.AreEqual(127, CampusPresentationTailRollupRefreshThrough127PresentationSpec.RefreshThroughBatch);
        }

        [Test]
        public void Manifest_127()
        {
            Assert.AreEqual(127, CampusPresentationTailRollupRefreshThrough127PresentationSpec.ManifestRefreshBatch);
        }
    }
}