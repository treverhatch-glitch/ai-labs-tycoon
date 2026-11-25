using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusRivalWorldTailIndexPresentationSpecTests
    {
        [Test]
        public void Tail_has_four_entries()
        {
            Assert.AreEqual(4, CampusRivalWorldTailIndexPresentationSpec.TailEntryCount);
        }

        [Test]
        public void Layer_counts_match_index_specs()
        {
            Assert.AreEqual(2, CampusRivalWorldTailIndexPresentationSpec.RivalFeelingLayers);
            Assert.AreEqual(2, CampusRivalWorldTailIndexPresentationSpec.WorldMapLayers);
        }

        [Test]
        public void Bloc_count_positive()
        {
            Assert.Greater(CampusRivalWorldTailIndexPresentationSpec.KnownBlocs, 0);
        }
    }
}