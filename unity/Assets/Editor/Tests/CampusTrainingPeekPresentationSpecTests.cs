using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusTrainingPeekPresentationSpecTests
    {
        [Test]
        public void Peek_band_and_split_bar()
        {
            Assert.NotNull(CampusIconCatalog.PathFor(CampusTrainingPeekPresentationSpec.TrainingIconId));
            Assert.AreEqual(8f, CampusTrainingPeekPresentationSpec.SplitBarHeightPixels);
        }
    }
}