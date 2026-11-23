using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusMascotRowHudPresentationSpecTests
    {
        [Test]
        public void Strip_fraction_matches_hud_layout()
        {
            Assert.AreEqual(CampusHudLayoutSpec.MascotStripHeightFraction,
                CampusMascotRowHudPresentationSpec.StripHeightFraction);
        }
    }
}