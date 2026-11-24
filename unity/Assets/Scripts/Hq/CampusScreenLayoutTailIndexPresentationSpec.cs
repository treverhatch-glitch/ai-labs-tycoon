namespace AiLabs.Unity.Hq
{
    /// <summary>Screen layout tail index (see SCREEN-LAYOUT-TAIL-INDEX.md).</summary>
    public static class CampusScreenLayoutTailIndexPresentationSpec
    {
        public const int TailEntryCount = 4;

        public static int FullScreenLayoutCount =>
            CampusFullScreenLayoutIndexPresentationSpec.DocumentedFullScreenLayouts;

        public static int HubTailRows =>
            CampusPresentationIndexHubTailPresentationSpec.TailEntryCount;

        public const string TailDoc = "SCREEN-LAYOUT-TAIL-INDEX.md";
        public const string LayoutIndexDoc = "SCREEN-LAYOUT-INDEX.md";
    }
}