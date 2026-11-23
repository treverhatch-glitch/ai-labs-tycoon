using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusTrainingPeekIndexPresentationSpecTests
    {
        [Test]
        public void Layer_count()
        {
            Assert.AreEqual(2, CampusTrainingPeekIndexPresentationSpec.DocumentedTrainingPeekLayers);
        }

        [Test]
        public void Peek_fractions_match_peek_spec()
        {
            Assert.AreEqual(
                CampusTrainingPeekPresentationSpec.PanelWidthFraction,
                CampusTrainingPeekIndexPresentationSpec.PeekWidthFraction);
            Assert.AreEqual(
                CampusTrainingPeekPresentationSpec.PanelHeightFraction,
                CampusTrainingPeekIndexPresentationSpec.PeekHeightFraction);
        }
    }
}