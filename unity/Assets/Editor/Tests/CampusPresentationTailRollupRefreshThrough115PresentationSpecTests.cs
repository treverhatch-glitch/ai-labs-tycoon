using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough115PresentationSpecTests
    {
        [Test]
        public void Refresh_through_115()
        {
            Assert.AreEqual(115, CampusPresentationTailRollupRefreshThrough115PresentationSpec.RefreshThroughBatch);
        }

        [Test]
        public void Tail_lanes_eleven()
        {
            Assert.AreEqual(11, CampusPresentationTailRollupRefreshThrough115PresentationSpec.TailLaneCount);
        }

        [Test]
        public void Manifest_115()
        {
            Assert.AreEqual(115, CampusPresentationTailRollupRefreshThrough115PresentationSpec.ManifestRefreshBatch);
        }
    }
}