using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough121PresentationSpecTests
    {
        [Test]
        public void Refresh_through_121()
        {
            Assert.AreEqual(121, CampusPresentationTailRollupRefreshThrough121PresentationSpec.RefreshThroughBatch);
        }

        [Test]
        public void Prior_118()
        {
            Assert.AreEqual(118, CampusPresentationTailRollupRefreshThrough121PresentationSpec.PriorRefreshThroughBatch);
        }

        [Test]
        public void Manifest_121()
        {
            Assert.AreEqual(121, CampusPresentationTailRollupRefreshThrough121PresentationSpec.ManifestRefreshBatch);
        }
    }
}