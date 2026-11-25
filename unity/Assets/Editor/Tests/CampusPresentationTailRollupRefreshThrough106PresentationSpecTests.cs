using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshThrough106PresentationSpecTests
    {
        [Test]
        public void Refresh_through_106()
        {
            Assert.AreEqual(106, CampusPresentationTailRollupRefreshThrough106PresentationSpec.RefreshThroughBatch);
        }

        [Test]
        public void Tail_lanes_eleven()
        {
            Assert.AreEqual(11, CampusPresentationTailRollupRefreshThrough106PresentationSpec.TailLaneCount);
        }

        [Test]
        public void Last_grouped_109()
        {
            Assert.AreEqual(109, CampusPresentationTailRollupRefreshThrough106PresentationSpec.LastRemoteGroupedBatch);
        }
    }
}