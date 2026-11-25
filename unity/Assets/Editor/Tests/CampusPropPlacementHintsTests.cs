using AiLabs.Rules.Config;
using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPropPlacementHintsTests
    {
        [Test]
        public void Compute_room_gets_rack_and_blades()
        {
            var props = CampusPropPlacementHints.DefaultPropsFor(RoomPurpose.Compute);
            Assert.Contains("prop-server-rack", props);
            Assert.GreaterOrEqual(props.Length, 3);
        }

        [Test]
        public void Every_prop_resolves_in_catalog()
        {
            foreach (string id in CampusPropPlacementHints.DefaultPropsFor(RoomPurpose.Research))
                Assert.NotNull(CampusPropCatalog.PathFor(id));
        }
    }
}