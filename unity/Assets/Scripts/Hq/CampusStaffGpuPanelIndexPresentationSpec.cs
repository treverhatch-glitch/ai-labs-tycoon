namespace AiLabs.Unity.Hq
{
    /// <summary>Staff/GPU panel index (see STAFF-GPU-PANEL-INDEX.md).</summary>
    public static class CampusStaffGpuPanelIndexPresentationSpec
    {
        public const int DocumentedStaffGpuLayers = 2;

        public static float RowHeightPixels => CampusStaffGpuPanelPresentationSpec.RowHeightPixels;

        public static float PanelWidthFraction => CampusStaffGpuPanelPresentationSpec.PanelWidthFraction;

        public static float PanelHeightFraction => CampusStaffGpuPanelPresentationSpec.PanelHeightFraction;

        public const string PanelDoc = "STAFF-GPU-PANEL-PRESENTATION.md";
        public const string RowDoc = "STAFF-ROW-PRESENTATION.md";
    }
}