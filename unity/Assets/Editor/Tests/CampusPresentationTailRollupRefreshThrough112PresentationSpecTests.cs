using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough112PresentationSpecTests
    {
        [Test]
        public void Refresh_through_112()
        {
            Assert.AreEqual(112, CampusPresentationTailRollupRefreshThrough112PresentationSpec.RefreshThroughBatch);
        }

        [Test]
        public void Tail_lanes_eleven()
        {
            Assert.AreEqual(11, CampusPresentationTailRollupRefreshThrough112PresentationSpec.TailLaneCount);
        }

        [Test]
        public void Manifest_refresh_aligns()
        {
            Assert.AreEqual(112, CampusPresentationTailRollupRefreshThrough112PresentationSpec.ManifestRefreshBatch);
        }
    }
}