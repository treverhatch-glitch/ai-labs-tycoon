using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusWorldMapScreenHudTailPresentationSpecTests
    {
        [Test]
        public void Map_and_card_fractions_sum_to_one()
        {
            var spec = CampusWorldMapScreenHudTailPresentationSpec.MapPlaneWidthFraction
                + CampusWorldMapScreenHudTailPresentationSpec.SiteCardWidthFraction;
            Assert.AreEqual(1f, spec, 0.001f);
        }
    }
}