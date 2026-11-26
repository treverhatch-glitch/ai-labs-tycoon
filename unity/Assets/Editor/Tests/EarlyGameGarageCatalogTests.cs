using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class EarlyGameGarageCatalogTests
    {
        [Test]
        public void Garage_catalog_lists_15_props_with_paths()
        {
            Assert.AreEqual(15, EarlyGameGarageCatalog.AllAssetIds.Count);
            foreach (string id in EarlyGameGarageCatalog.AllAssetIds)
                Assert.IsNotNull(EarlyGameGarageCatalog.PathFor(id));
        }

        [Test]
        public void Staff_catalog_lists_8_roles()
        {
            Assert.AreEqual(8, EarlyGameStaffCatalog.AllAssetIds.Count);
        }

        [Test]
        public void Hud_catalog_lists_12_icons()
        {
            Assert.AreEqual(12, EarlyGameHudCatalog.HudIconIds.Count);
        }

        [Test]
        public void Wisp_catalog_lists_5_moods()
        {
            Assert.AreEqual(5, EarlyGameWispCatalog.WispMoodIds.Count);
        }

        [Test]
        public void Texture_catalog_lists_8_swatches()
        {
            Assert.AreEqual(8, EarlyGameTextureCatalog.TextureIds.Count);
        }
    }
}