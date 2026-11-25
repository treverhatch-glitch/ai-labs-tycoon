using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusHqMascotStripPresentationSpecTests
    {
        [Test]
        public void Strip_fraction_and_row()
        {
            Assert.AreEqual(0.18f, CampusHqMascotStripPresentationSpec.StripHeightFraction);
            Assert.AreEqual(48f, CampusHqMascotStripPresentationSpec.RowHeightPixels);
        }
    }
}