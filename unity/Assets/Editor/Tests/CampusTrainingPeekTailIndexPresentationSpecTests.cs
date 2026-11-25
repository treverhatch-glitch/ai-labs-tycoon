using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusTrainingPeekTailIndexPresentationSpecTests
    {
        [Test]
        public void Tail_anchors_three()
        {
            Assert.AreEqual(3, CampusTrainingPeekTailIndexPresentationSpec.TailAnchorCount);
        }

        [Test]
        public void Training_hub_two_layers()
        {
            Assert.AreEqual(2, CampusTrainingPeekTailIndexPresentationSpec.TrainingHubLayers);
        }

        [Test]
        public void Staff_tail_anchors_three()
        {
            Assert.AreEqual(3, CampusTrainingPeekTailIndexPresentationSpec.StaffTailAnchors);
        }
    }
}