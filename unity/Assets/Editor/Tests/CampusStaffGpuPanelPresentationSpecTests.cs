using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusStaffGpuPanelPresentationSpecTests
    {
        [Test]
        public void Panel_icons_and_row_height()
        {
            Assert.NotNull(CampusIconCatalog.PathFor(CampusStaffGpuPanelPresentationSpec.StaffIconId));
            Assert.NotNull(CampusIconCatalog.PathFor(CampusStaffGpuPanelPresentationSpec.ComputeIconId));
            Assert.AreEqual(36f, CampusStaffGpuPanelPresentationSpec.RowHeightPixels);
        }
    }
}