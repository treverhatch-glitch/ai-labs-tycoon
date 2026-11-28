using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusTopBarTokenIndexPresentationSpecTests
    {
        [Test]
        public void Top_bar_height_matches_hud_layout()
        {
            Assert.AreEqual(CampusHudLayoutSpec.TopBarHeight, CampusTopBarTokenIndexPresentationSpec.TopBarHeightPixels);
        }

        [Test]
        public void Chrome_slot_count()
        {
            Assert.AreEqual(4, CampusTopBarTokenIndexPresentationSpec.DocumentedChromeSlots);
        }

    }
}