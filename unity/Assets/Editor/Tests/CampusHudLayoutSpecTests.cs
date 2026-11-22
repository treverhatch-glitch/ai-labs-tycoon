using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusHudLayoutSpecTests
    {
        [Test]
        public void Top_bar_and_cta_sizes_match_doc()
        {
            Assert.AreEqual(72f, CampusHudLayoutSpec.TopBarHeight);
            Assert.Greater(CampusHudLayoutSpec.AdvanceWeekWidth, 44f);
        }

        [Test]
        public void Rivals_split_matches_layout_doc()
        {
            Assert.AreEqual(1f, CampusHudLayoutSpec.RivalsListWidthFraction + CampusHudLayoutSpec.RivalsLogWidthFraction, 0.001f);
        }
    }
}