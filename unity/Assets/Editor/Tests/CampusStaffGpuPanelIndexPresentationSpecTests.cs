using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusStaffGpuPanelIndexPresentationSpecTests
    {
        [Test]
        public void Layer_count()
        {
            Assert.AreEqual(2, CampusStaffGpuPanelIndexPresentationSpec.DocumentedStaffGpuLayers);
        }

        [Test]
        public void Row_height_matches_panel_spec()
        {
            Assert.AreEqual(
                CampusStaffGpuPanelPresentationSpec.RowHeightPixels,
                CampusStaffGpuPanelIndexPresentationSpec.RowHeightPixels);
        }

        [Test]
        public void Panel_fractions_match_panel_spec()
        {
            Assert.AreEqual(
                CampusStaffGpuPanelPresentationSpec.PanelWidthFraction,
                CampusStaffGpuPanelIndexPresentationSpec.PanelWidthFraction);
            Assert.AreEqual(
                CampusStaffGpuPanelPresentationSpec.PanelHeightFraction,
                CampusStaffGpuPanelIndexPresentationSpec.PanelHeightFraction);
        }
    }
}