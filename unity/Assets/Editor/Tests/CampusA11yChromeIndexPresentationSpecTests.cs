using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusA11yChromeIndexPresentationSpecTests
    {
        [Test]
        public void A11y_layer_count()
        {
            Assert.AreEqual(2, CampusA11yChromeIndexPresentationSpec.DocumentedA11yLayers);
        }

        [Test]
        public void Min_hit_target_matches_focus_ring_spec()
        {
            Assert.AreEqual(
                CampusFocusRingPresentationSpec.MinHitTargetPixels,
                CampusA11yChromeIndexPresentationSpec.MinHitTargetPixels);
        }

        [Test]
        public void Capture_top_bar_matches_snapshot_spec()
        {
            Assert.AreEqual(
                CampusCaptureHudSnapshotPresentationSpec.TopBarHeightPixels,
                CampusA11yChromeIndexPresentationSpec.CaptureTopBarHeightPixels);
        }
    }
}