using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusExplainableHudIndexPresentationSpecTests
    {
        [Test]
        public void Breakdown_drawer_count()
        {
            Assert.AreEqual(2, CampusExplainableHudIndexPresentationSpec.DocumentedBreakdownDrawers);
        }

        [Test]
        public void Shared_row_height_matches_hq_breakdown()
        {
            Assert.AreEqual(
                CampusBreakdownPresentationSpec.PartRowHeightPixels,
                CampusExplainableHudIndexPresentationSpec.SharedPartRowHeightPixels);
        }

        [Test]
        public void Research_row_height_matches_index()
        {
            Assert.AreEqual(
                CampusExplainableHudIndexPresentationSpec.SharedPartRowHeightPixels,
                CampusResearchBreakdownPresentationSpec.PartRowHeightPixels);
        }
    }
}