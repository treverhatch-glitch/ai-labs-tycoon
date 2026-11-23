using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusHudDateWeekPresentationSpecTests
    {
        [Test]
        public void Week_button_meets_focus_ring_minimum()
        {
            Assert.GreaterOrEqual(CampusHudDateWeekPresentationSpec.WeekButtonMinPixels, 44f);
        }
    }
}