using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough133PresentationSpecTests
    {
        [Test]
        public void Refresh_through_133()
        {
            Assert.AreEqual(133, CampusPresentationTailRollupRefreshThrough133PresentationSpec.RefreshThroughBatch);
        }

        [Test]
        public void Manifest_133()
        {
            Assert.AreEqual(133, CampusPresentationTailRollupRefreshThrough133PresentationSpec.ManifestRefreshBatch);
        }
    }
}