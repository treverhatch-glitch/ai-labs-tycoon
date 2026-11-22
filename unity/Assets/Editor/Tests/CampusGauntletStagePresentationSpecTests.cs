using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGauntletStagePresentationSpecTests
    {
        [Test]
        public void Stage_props_resolve_in_catalog()
        {
            Assert.NotNull(CampusPropCatalog.PathFor(CampusGauntletStagePresentationSpec.LecternPropId));
            Assert.NotNull(CampusPropCatalog.PathFor(CampusGauntletStagePresentationSpec.BackdropPropId));
        }
    }
}