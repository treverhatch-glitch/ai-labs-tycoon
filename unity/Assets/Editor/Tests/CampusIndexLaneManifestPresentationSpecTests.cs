using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIndexLaneManifestPresentationSpecTests
    {
        [Test]
        public void Entry_count_matches_batch_span()
        {
            int span = CampusIndexLaneManifestPresentationSpec.Post60IndexBatchEnd
                       - CampusIndexLaneManifestPresentationSpec.Post60IndexBatchStart + 1;
            Assert.AreEqual(span, CampusIndexLaneManifestPresentationSpec.Post60IndexEntryCount);
        }

        [Test]
        public void Hq_indexed_bands_positive()
        {
            Assert.Greater(CampusIndexLaneManifestPresentationSpec.HqCompletionIndexedBands, 0);
        }
    }
}