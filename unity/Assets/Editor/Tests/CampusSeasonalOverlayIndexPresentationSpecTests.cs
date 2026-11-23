using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusSeasonalOverlayIndexPresentationSpecTests
    {
        [Test]
        public void Overlay_count()
        {
            Assert.AreEqual(1, CampusSeasonalOverlayIndexPresentationSpec.DocumentedSeasonalOverlays);
        }

        [Test]
        public void Gauntlet_fade_matches_stage_spec()
        {
            Assert.AreEqual(
                CampusGauntletStagePresentationSpec.OverlayFadeSeconds,
                CampusSeasonalOverlayIndexPresentationSpec.GauntletFadeSeconds,
                0.001f);
        }
    }
}