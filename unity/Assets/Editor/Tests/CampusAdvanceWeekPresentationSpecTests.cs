using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAdvanceWeekPresentationSpecTests
    {
        [Test]
        public void Advance_week_button_size()
        {
            Assert.AreEqual(200f, CampusAdvanceWeekPresentationSpec.ButtonWidthPixels);
            Assert.AreEqual(48f, CampusAdvanceWeekPresentationSpec.ButtonHeightPixels);
        }
    }
}