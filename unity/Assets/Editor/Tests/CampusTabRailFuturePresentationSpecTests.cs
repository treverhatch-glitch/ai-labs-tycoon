using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusTabRailFuturePresentationSpecTests
    {
        [Test]
        public void Future_rail_width_and_tabs()
        {
            Assert.AreEqual(64f, CampusTabRailFuturePresentationSpec.RailWidthPixels);
            Assert.AreEqual(5, CampusTabRailFuturePresentationSpec.TabCount);
        }
    }
}