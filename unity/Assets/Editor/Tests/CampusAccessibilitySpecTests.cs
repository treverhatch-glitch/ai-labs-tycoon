using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAccessibilitySpecTests
    {
        [Test]
        public void Advance_week_button_meets_min_hit_target()
        {
            float minNorm = CampusAccessibilitySpec.MinHitTargetNormalized;
            float buttonH = CampusHudLayoutSpec.AdvanceWeekHeight / CampusAccessibilitySpec.DesignHeightPixels;
            Assert.GreaterOrEqual(buttonH, minNorm * 0.99f);
        }

        [Test]
        public void Min_hit_target_at_least_44px()
        {
            Assert.GreaterOrEqual(CampusAccessibilitySpec.MinHitTargetPixels, 44f);
        }
    }
}