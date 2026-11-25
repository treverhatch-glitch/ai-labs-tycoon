using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusHqMascotStripIndexPresentationSpecTests
    {
        [Test]
        public void Layer_count()
        {
            Assert.AreEqual(2, CampusHqMascotStripIndexPresentationSpec.DocumentedMascotStripLayers);
        }

        [Test]
        public void Max_mascots_matches_strip_spec()
        {
            Assert.AreEqual(
                CampusHqMascotStripPresentationSpec.MaxVisibleMascots,
                CampusHqMascotStripIndexPresentationSpec.MaxVisibleMascots);
        }

        [Test]
        public void Strip_height_matches_strip_spec()
        {
            Assert.AreEqual(
                CampusHqMascotStripPresentationSpec.StripHeightFraction,
                CampusHqMascotStripIndexPresentationSpec.StripHeightFraction);
        }
    }
}