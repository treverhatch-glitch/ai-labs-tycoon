using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusHqMascotStripTailIndexPresentationSpecTests
    {
        [Test]
        public void Tail_anchors_three()
        {
            Assert.AreEqual(3, CampusHqMascotStripTailIndexPresentationSpec.TailAnchorCount);
        }

        [Test]
        public void Mascot_hub_two_layers()
        {
            Assert.AreEqual(2, CampusHqMascotStripTailIndexPresentationSpec.MascotHubLayers);
        }

        [Test]
        public void Max_visible_mascots_positive()
        {
            Assert.Greater(CampusHqMascotStripTailIndexPresentationSpec.MaxVisibleMascots, 0);
        }
    }
}