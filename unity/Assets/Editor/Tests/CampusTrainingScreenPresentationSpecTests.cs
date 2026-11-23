using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusTrainingScreenPresentationSpecTests
    {
        [Test]
        public void Training_screen_split_fractions()
        {
            Assert.AreEqual(0.60f, CampusTrainingScreenPresentationSpec.RunQueueWidthFraction);
            Assert.AreEqual(3, CampusTrainingScreenPresentationSpec.SplitSliderCount);
        }
    }
}