using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusWorldMapEraSystemsBibleTailPresentationSpecTests
    {
        [Test]
        public void Map_fraction_positive()
        {
            Assert.Greater(CampusWorldMapEraSystemsBibleTailPresentationSpec.MapPlaneWidthFraction, 0f);
        }
    }
}