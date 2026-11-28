using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusRoomBuildPresentationSpecTests
    {
        [Test]
        public void Build_panel_fractions_and_icon()
        {
            Assert.NotNull(CampusIconCatalog.PathFor(CampusRoomBuildPresentationSpec.BuildIconId));
            Assert.Greater(CampusRoomBuildPresentationSpec.LotViewportWidthFraction, 0.5f);
            Assert.LessOrEqual(
                CampusRoomBuildPresentationSpec.LotViewportWidthFraction
                    + CampusRoomBuildPresentationSpec.BuildPanelWidthFraction,
                1.01f);
        }
    }
}