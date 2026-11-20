using AiLabs.Rules.State;
using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class ModelMascotPresentationTests
    {
        [Test]
        public void Each_mood_has_distinct_scale()
        {
            float h = ModelMascotPresentation.ForMood(ModelMood.Healthy).Scale;
            float o = ModelMascotPresentation.ForMood(ModelMood.Overworked).Scale;
            float g = ModelMascotPresentation.ForMood(ModelMood.Glitching).Scale;
            Assert.AreNotEqual(h, o);
            Assert.AreNotEqual(h, g);
        }
    }
}