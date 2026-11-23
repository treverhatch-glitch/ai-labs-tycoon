using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusStaffPresentationIndexPresentationSpecTests
    {
        [Test]
        public void Staff_layer_count()
        {
            Assert.AreEqual(2, CampusStaffPresentationIndexPresentationSpec.DocumentedStaffLayers);
        }

        [Test]
        public void Shared_row_height_matches_panel_spec()
        {
            Assert.AreEqual(
                CampusStaffGpuPanelPresentationSpec.RowHeightPixels,
                CampusStaffPresentationIndexPresentationSpec.SharedRowHeightPixels);
        }
    }
}