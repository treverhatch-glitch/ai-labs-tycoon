using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough130PresentationSpecTests
    {
        [Test]
        public void Refresh_through_130()
        {
            Assert.AreEqual(130, CampusPresentationTailRollupRefreshThrough130PresentationSpec.RefreshThroughBatch);
        }

        [Test]
        public void Manifest_130()
        {
            Assert.AreEqual(130, CampusPresentationTailRollupRefreshThrough130PresentationSpec.ManifestRefreshBatch);
        }
    }
}