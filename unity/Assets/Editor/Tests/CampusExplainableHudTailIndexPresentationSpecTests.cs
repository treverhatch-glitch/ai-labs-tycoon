using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusExplainableHudTailIndexPresentationSpecTests
    {
        [Test]
        public void Tail_has_four_entries()
        {
            Assert.AreEqual(4, CampusExplainableHudTailIndexPresentationSpec.TailEntryCount);
        }

        [Test]
        public void Row_height_matches_index()
        {
            Assert.AreEqual(
                CampusExplainableHudIndexPresentationSpec.SharedPartRowHeightPixels,
                CampusExplainableHudTailIndexPresentationSpec.SharedPartRowHeightPixels);
        }

        [Test]
        public void Room_build_layers_positive()
        {
            Assert.Greater(CampusExplainableHudTailIndexPresentationSpec.RoomBuildLayers, 0);
        }
    }
}