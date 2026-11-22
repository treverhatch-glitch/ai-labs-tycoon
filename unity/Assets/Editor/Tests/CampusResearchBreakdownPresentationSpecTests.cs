using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusResearchBreakdownPresentationSpecTests
    {
        [Test]
        public void Drawer_width_matches_research_panel_spec()
        {
            Assert.AreEqual(0.38f, CampusResearchBreakdownPresentationSpec.DetailDrawerWidthFraction, 0.001f);
        }

        [Test]
        public void Header_icon_resolves()
        {
            Assert.NotNull(CampusIconCatalog.PathFor(CampusResearchBreakdownPresentationSpec.HeaderIconId));
        }
    }
}