using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusHudEraTailIndexPresentationSpecTests
    {
        [Test]
        public void Tail_span_covers_four_batches()
        {
            Assert.AreEqual(4, CampusHudEraTailIndexPresentationSpec.EraTailEntryCount);
        }

        [Test]
        public void Era_label_width_positive()
        {
            Assert.Greater(CampusHudEraTailIndexPresentationSpec.EraLabelWidthPixels, 0f);
        }

        [Test]
        public void Skin_count_matches_preview_spec()
        {
            Assert.AreEqual(
                CampusEraSkinPreviewPresentationSpec.MinEraSkinCount,
                CampusHudEraTailIndexPresentationSpec.MinEraSkinCount);
        }

        [Test]
        public void Seasonal_slots_positive()
        {
            Assert.Greater(CampusHudEraTailIndexPresentationSpec.SeasonalOverlaySlots, 0);
        }
    }
}