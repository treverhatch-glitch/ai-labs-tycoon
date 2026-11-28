using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusTabIconBindingsTests
    {
        [Test]
        public void Every_tab_has_icon_path()
        {
            foreach (string tabId in CampusTabRailMock.TabIds)
            {
                string path = CampusTabIconBindings.IconPathForTab(tabId);
                StringAssert.StartsWith("Assets/Art/Generated/ui-icons/", path);
                StringAssert.EndsWith(".svg", path);
            }
        }

        [Test]
        public void World_tab_uses_world_map_tokens()
        {
            Assert.AreEqual(CampusWorldMapTokens.WorldIconId, CampusTabIconBindings.IconIdForTab("world"));
        }
    }
}