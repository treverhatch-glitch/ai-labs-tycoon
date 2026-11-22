namespace AiLabs.Unity.Hq
{
    /// <summary>Tab rail id → HUD icon asset id (see TAB-RAIL-INTEGRATION.md).</summary>
    public static class CampusTabIconBindings
    {
        public static string IconIdForTab(string tabId)
        {
            switch (tabId)
            {
                case "hq": return "icon-hq";
                case "research": return "icon-research";
                case "training": return "icon-training";
                case "rivals": return "icon-rivals";
                case "world": return "icon-world";
                default: return "icon-hq";
            }
        }

        public static string IconPathForTab(string tabId) =>
            CampusIconCatalog.PathFor(IconIdForTab(tabId));
    }
}