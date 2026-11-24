using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusStaffPresentationTailIndexPresentationSpecTests
    {
        [Test]
        public void Tail_anchors_three()
        {
            Assert.AreEqual(3, CampusStaffPresentationTailIndexPresentationSpec.TailAnchorCount);
        }

        [Test]
        public void Staff_hub_and_gpu_layers_positive()
        {
            Assert.Greater(CampusStaffPresentationTailIndexPresentationSpec.StaffHubLayers, 0);
            Assert.Greater(CampusStaffPresentationTailIndexPresentationSpec.StaffGpuIndexLayers, 0);
        }

        [Test]
        public void Rollup_lanes_match_tail_rollup()
        {
            Assert.AreEqual(7, CampusStaffPresentationTailIndexPresentationSpec.PostTailRollupLanes);
        }
    }
}