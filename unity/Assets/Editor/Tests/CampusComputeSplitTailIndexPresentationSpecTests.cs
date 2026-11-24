using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusComputeSplitTailIndexPresentationSpecTests
    {
        [Test]
        public void Tail_anchors_three()
        {
            Assert.AreEqual(3, CampusComputeSplitTailIndexPresentationSpec.TailAnchorCount);
        }

        [Test]
        public void Compute_hub_two_layers()
        {
            Assert.AreEqual(2, CampusComputeSplitTailIndexPresentationSpec.ComputeHubLayers);
        }

        [Test]
        public void Slider_count_positive()
        {
            Assert.Greater(CampusComputeSplitTailIndexPresentationSpec.SliderCount, 0);
        }
    }
}