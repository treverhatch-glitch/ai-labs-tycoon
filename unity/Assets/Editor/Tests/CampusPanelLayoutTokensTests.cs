using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPanelLayoutTokensTests
    {
        [Test]
        public void Research_columns_sum_to_one()
        {
            Assert.AreEqual(1f,
                CampusPanelLayoutTokens.ResearchTreeFraction + CampusPanelLayoutTokens.ResearchDetailFraction,
                0.001f);
        }

        [Test]
        public void Rivals_split_matches_hud_spec()
        {
            Assert.AreEqual(CampusHudLayoutSpec.RivalsListWidthFraction, CampusPanelLayoutTokens.RivalsListFraction);
        }
    }
}