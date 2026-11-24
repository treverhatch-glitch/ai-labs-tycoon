using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusHqLotViewportIndexPresentationSpecTests
    {
        [Test]
        public void Fraction_matches_viewport_spec()
        {
            Assert.AreEqual(
                CampusHqLotViewportPresentationSpec.ViewportWidthFraction,
                CampusHqLotViewportIndexPresentationSpec.ViewportWidthFraction);
        }
    }
}