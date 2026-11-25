using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class TrainingRunFormatterTests
    {
        [Test]
        public void Progress_permille_clamps()
        {
            var model = new AiLabs.Rules.State.ModelState();
            model.RequiredCompute = 1000;
            model.ProgressCompute = 250;
            Assert.AreEqual(250, TrainingRunFormatter.ProgressPermille(model));
            model.ProgressCompute = 5000;
            Assert.AreEqual(1000, TrainingRunFormatter.ProgressPermille(model));
        }
    }
}