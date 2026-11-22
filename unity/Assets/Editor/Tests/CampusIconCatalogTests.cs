using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusIconCatalogTests
    {
        [Test]
        public void Hud_icons_include_staff_and_danger()
        {
            Assert.AreEqual(10, CampusIconCatalog.AllIconIds.Length);
            Assert.NotNull(CampusIconCatalog.PathFor("icon-staff"));
            Assert.NotNull(CampusIconCatalog.PathFor("icon-danger"));
        }
    }
}