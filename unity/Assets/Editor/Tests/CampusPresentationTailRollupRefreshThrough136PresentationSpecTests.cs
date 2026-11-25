using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough136PresentationSpecTests
    {
        [Test]
        public void Refresh_136_manifest_136()
        {
            Assert.AreEqual(136, CampusPresentationTailRollupRefreshThrough136PresentationSpec.RefreshThroughBatch);
            Assert.AreEqual(136, CampusPresentationTailRollupRefreshThrough136PresentationSpec.ManifestRefreshBatch);
        }
    }
}