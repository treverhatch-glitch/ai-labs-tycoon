using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusWorldMapHostPresentationSpecTests
    {
        [Test]
        public void World_icon_and_blocs_resolve()
        {
            Assert.NotNull(CampusIconCatalog.PathFor(CampusWorldMapHostPresentationSpec.WorldIconId));
            Assert.GreaterOrEqual(CampusWorldMapHostPresentationSpec.KnownBlocCount, 4);
        }
    }
}