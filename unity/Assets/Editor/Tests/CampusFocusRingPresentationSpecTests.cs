using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusFocusRingPresentationSpecTests
    {
        [Test]
        public void Hit_target_meets_a11y_minimum()
        {
            Assert.GreaterOrEqual(CampusFocusRingPresentationSpec.MinHitTargetPixels, 44f);
        }
    }
}