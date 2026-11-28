using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusSeasonalA11yChromeTailPresentationSpecTests
    {
        [Test]
        public void Seasonal_overlays_two()
        {
            Assert.AreEqual(2, CampusSeasonalA11yChromeTailPresentationSpec.SeasonalOverlays);
        }
    }
}