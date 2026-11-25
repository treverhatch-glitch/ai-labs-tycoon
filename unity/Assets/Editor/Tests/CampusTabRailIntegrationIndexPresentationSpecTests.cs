using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusTabRailIntegrationIndexPresentationSpecTests
    {
        [Test]
        public void Layer_count()
        {
            Assert.AreEqual(3, CampusTabRailIntegrationIndexPresentationSpec.DocumentedTabRailLayers);
        }

        [Test]
        public void Tab_count_matches_mock()
        {
            Assert.AreEqual(
                CampusTabRailMock.TabIds.Length,
                CampusTabRailIntegrationIndexPresentationSpec.TabCount);
        }

        [Test]
        public void Rail_width_matches_future_spec()
        {
            Assert.AreEqual(
                CampusTabRailFuturePresentationSpec.RailWidthPixels,
                CampusTabRailIntegrationIndexPresentationSpec.RailWidthPixels);
        }
    }
}