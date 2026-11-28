namespace AiLabs.Unity.Hq
{
    /// <summary>GO packet refresh for post-60 index lane (see GO-PACKET-INDEX-LANE-REFRESH.md).</summary>
    public static class CampusGoPacketIndexLaneRefreshPresentationSpec
    {
        public static int Post60FirstBatch => CampusIndexLaneManifestPresentationSpec.Post60IndexBatchStart;

        public static int Post60LastBatch => CampusIndexLaneManifestPresentationSpec.Post60IndexBatchEnd;

        public static int MinSpacingMinutes => CampusGoPacketTemplatePresentationSpec.MinSpacingMinutes;

        public const string RefreshDoc = "GO-PACKET-INDEX-LANE-REFRESH.md";
        public const string TemplateDoc = "GO-PACKET-TEMPLATE.md";
    }
}