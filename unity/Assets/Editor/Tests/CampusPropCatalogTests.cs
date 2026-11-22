using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPropCatalogTests
    {
        [Test]
        public void Catalog_lists_25_props_with_paths()
        {
            Assert.AreEqual(25, CampusPropCatalog.AllAssetIds.Count);
            foreach (string id in CampusPropCatalog.AllAssetIds)
            {
                string path = CampusPropCatalog.PathFor(id);
                Assert.IsNotNull(path);
                StringAssert.Contains(id, path);
            }
        }

        [Test]
        public void Legacy_ids_still_resolve()
        {
            Assert.NotNull(CampusPropCatalog.PathFor(CampusPropIds.Desk));
            Assert.NotNull(CampusPropCatalog.PathFor(CampusPropIds.RivalBadge));
        }
    }
}