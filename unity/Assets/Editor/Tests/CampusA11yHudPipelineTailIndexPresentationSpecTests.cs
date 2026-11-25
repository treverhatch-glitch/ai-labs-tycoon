using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusA11yHudPipelineTailIndexPresentationSpecTests
    {
        [Test]
        public void Hit_target_positive()
        {
            Assert.Greater(CampusA11yHudPipelineTailIndexPresentationSpec.MinHitTargetPixels, 0);
        }
    }
}