using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusHqStageStackCompletionTailIndexPresentationSpecTests
    {
        [Test]
        public void Tail_anchors_three()
        {
            Assert.AreEqual(3, CampusHqStageStackCompletionTailIndexPresentationSpec.TailAnchorCount);
        }

        [Test]
        public void Indexed_bands_five()
        {
            Assert.AreEqual(5, CampusHqStageStackCompletionTailIndexPresentationSpec.IndexedHqBands);
        }

        [Test]
        public void Total_bands_not_below_indexed()
        {
            Assert.GreaterOrEqual(
                CampusHqStageStackCompletionTailIndexPresentationSpec.TotalHqBands,
                CampusHqStageStackCompletionTailIndexPresentationSpec.IndexedHqBands);
        }
    }
}