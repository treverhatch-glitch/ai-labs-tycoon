using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough118PresentationSpecTests
    {
        [Test]
        public void Refresh_through_118()
        {
            Assert.AreEqual(118, CampusPresentationTailRollupRefreshThrough118PresentationSpec.RefreshThroughBatch);
        }

        [Test]
        public void Prior_115()
        {
            Assert.AreEqual(115, CampusPresentationTailRollupRefreshThrough118PresentationSpec.PriorRefreshThroughBatch);
        }

        [Test]
        public void Manifest_118()
        {
            Assert.AreEqual(118, CampusPresentationTailRollupRefreshThrough118PresentationSpec.ManifestRefreshBatch);
        }
    }
}