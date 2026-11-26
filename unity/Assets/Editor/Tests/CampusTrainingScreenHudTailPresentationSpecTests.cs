using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusTrainingScreenHudTailPresentationSpecTests
    {
        [Test]
        public void Run_queue_fraction()
        {
            Assert.AreEqual(0.60f, CampusTrainingScreenHudTailPresentationSpec.RunQueueWidthFraction);
        }
    }
}