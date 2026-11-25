using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusWorldMapTokensTests
    {
        [Test]
        public void Map_props_resolve_in_catalog()
        {
            Assert.NotNull(CampusPropCatalog.PathFor(CampusWorldMapTokens.PinPropId));
            Assert.NotNull(CampusPropCatalog.PathFor(CampusWorldMapTokens.BlocTilePropId));
        }

        [Test]
        public void Bloc_ids_non_empty()
        {
            Assert.GreaterOrEqual(CampusWorldMapTokens.KnownBlocIds.Length, 4);
        }
    }
}