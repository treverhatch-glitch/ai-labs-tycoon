using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusCaptureHudTailIndexPresentationSpecTests
    {
        [Test]
        public void Tail_has_four_entries()
        {
            Assert.AreEqual(4, CampusCaptureHudTailIndexPresentationSpec.TailEntryCount);
        }

        [Test]
        public void Hit_target_matches_a11y_index()
        {
            Assert.AreEqual(
                CampusA11yChromeIndexPresentationSpec.MinHitTargetPixels,
                CampusCaptureHudTailIndexPresentationSpec.MinHitTargetPixels);
        }

        [Test]
        public void Capture_bar_height_positive()
        {
            Assert.Greater(CampusCaptureHudTailIndexPresentationSpec.CaptureTopBarHeightPixels, 0);
        }
    }
}