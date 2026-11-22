using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIconCatalogTests
    {
        [Test]
        public void Eight_hud_icons()
        {
            Assert.AreEqual(8, CampusIconCatalog.AllIconIds.Length);
            Assert.IsTrue(CampusIconCatalog.PathFor("icon-hq").EndsWith(".svg"));
        }
    }
}