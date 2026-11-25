using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationTailRollupRefreshPresentationSpecTests
    {
        [Test]
        public void Tail_lanes_eleven()
        {
            Assert.AreEqual(11, CampusPresentationTailRollupRefreshPresentationSpec.DocumentedTailLanes);
        }

        [Test]
        public void Prior_seven_lanes()
        {
            Assert.AreEqual(7, CampusPresentationTailRollupRefreshPresentationSpec.PriorTailLaneCount);
        }

        [Test]
        public void Last_grouped_tail_98()
        {
            Assert.AreEqual(98, CampusPresentationTailRollupRefreshPresentationSpec.LastGroupedTailBatch);
        }
    }
}