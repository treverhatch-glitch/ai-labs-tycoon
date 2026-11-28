using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusComputeSplitIndexPresentationSpecTests
    {
        [Test]
        public void Layer_count()
        {
            Assert.AreEqual(2, CampusComputeSplitIndexPresentationSpec.DocumentedComputeSplitLayers);
        }

        [Test]
        public void Bar_height_matches_peek_spec()
        {
            Assert.AreEqual(
                CampusTrainingPeekPresentationSpec.SplitBarHeightPixels,
                CampusComputeSplitIndexPresentationSpec.SplitBarHeightPixels);
        }

        [Test]
        public void Slider_count_matches_split_spec()
        {
            Assert.AreEqual(
                CampusComputeSplitPresentationSpec.SliderCount,
                CampusComputeSplitIndexPresentationSpec.SliderCount);
        }
    }
}