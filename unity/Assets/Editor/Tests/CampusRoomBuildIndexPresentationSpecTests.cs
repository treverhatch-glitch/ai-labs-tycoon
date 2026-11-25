using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusRoomBuildIndexPresentationSpecTests
    {
        [Test]
        public void Layer_count()
        {
            Assert.AreEqual(2, CampusRoomBuildIndexPresentationSpec.DocumentedRoomBuildLayers);
        }

        [Test]
        public void Fractions_match_room_build_spec()
        {
            Assert.AreEqual(
                CampusRoomBuildPresentationSpec.BuildPanelWidthFraction,
                CampusRoomBuildIndexPresentationSpec.BuildPanelWidthFraction);
            Assert.AreEqual(
                CampusRoomBuildPresentationSpec.BuildPanelHeightFraction,
                CampusRoomBuildIndexPresentationSpec.BuildPanelHeightFraction);
            Assert.AreEqual(
                CampusRoomBuildPresentationSpec.LotViewportWidthFraction,
                CampusRoomBuildIndexPresentationSpec.LotViewportWidthFraction);
        }
    }
}