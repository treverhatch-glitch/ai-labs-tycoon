using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusWorldMapFreezePresentationSpecTests
    {
        [Test]
        public void Freeze_assets_resolve_in_catalog()
        {
            Assert.NotNull(CampusIconCatalog.PathFor(CampusWorldMapFreezePresentationSpec.DangerIconId));
            Assert.NotNull(CampusPropCatalog.PathFor(CampusWorldMapFreezePresentationSpec.PinPropId));
        }
    }
}