namespace AiLabs.Unity.Hq
{
    /// <summary>Staff tail cross-links after presentation rollup (see STAFF-PRESENTATION-TAIL-INDEX.md).</summary>
    public static class CampusStaffPresentationTailIndexPresentationSpec
    {
        public const int TailAnchorCount = 3;

        public static int StaffHubLayers =>
            CampusStaffPresentationIndexPresentationSpec.DocumentedStaffLayers;

        public static int StaffGpuIndexLayers =>
            CampusStaffGpuPanelIndexPresentationSpec.DocumentedStaffGpuLayers;

        public static int PostTailRollupLanes =>
            CampusPresentationTailRollupPresentationSpec.DocumentedTailLanes;

        public const string TailIndexDoc = "STAFF-PRESENTATION-TAIL-INDEX.md";

        public const int DocumentedPresentationBatch = 248;
    }
}