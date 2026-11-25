using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestTailPresentationSpecTests
    {
        [Test]
        public void Tail_span_matches_entry_count()
        {
            int span = CampusIndexLaneManifestTailPresentationSpec.TailBatchEnd
                       - CampusIndexLaneManifestTailPresentationSpec.TailBatchStart + 1;
            Assert.AreEqual(span, CampusIndexLaneManifestTailPresentationSpec.TailEntryCount);
        }

        [Test]
        public void Extended_lane_includes_manifest_and_tail()
        {
            int span = CampusIndexLaneManifestTailPresentationSpec.ExtendedLaneBatchEnd
                       - CampusIndexLaneManifestTailPresentationSpec.ExtendedLaneBatchStart + 1;
            Assert.AreEqual(span, CampusIndexLaneManifestTailPresentationSpec.ExtendedLaneEntryCount);
        }

        [Test]
        public void Tail_starts_after_base_manifest()
        {
            Assert.AreEqual(
                CampusIndexLaneManifestTailPresentationSpec.BaseManifestEnd + 1,
                CampusIndexLaneManifestTailPresentationSpec.TailBatchStart);
        }

        [Test]
        public void Indexed_hq_bands_five_after_lot_viewport()
        {
            Assert.AreEqual(5, CampusIndexLaneManifestTailPresentationSpec.IndexedHqBands);
        }
    }
}