using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusSeasonalOverlayIndexPresentationSpecTests
    {
        [Test]
        public void Overlay_count()
        {
            Assert.AreEqual(2, CampusSeasonalOverlayIndexPresentationSpec.DocumentedSeasonalOverlays);
        }

        [Test]
        public void Gauntlet_fade_matches_stage_spec()
        {
            Assert.AreEqual(
                CampusGauntletStagePresentationSpec.OverlayFadeSeconds,
                CampusSeasonalOverlayIndexPresentationSpec.GauntletFadeSeconds,
                0.001f);
        }

        [Test]
        public void Freeze_pin_alpha_matches_world_map_spec()
        {
            Assert.AreEqual(
                CampusWorldMapFreezePresentationSpec.FrozenPinAlpha,
                CampusSeasonalOverlayIndexPresentationSpec.WorldMapFrozenPinAlpha,
                0.001f);
        }
    }
}