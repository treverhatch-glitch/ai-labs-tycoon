namespace AiLabs.Unity.Hq
{
    /// <summary>GO packet tail through batch 139 (see GO-PACKET-TAIL-REFRESH-139.md).</summary>
    public static class CampusGoPacketTailRefreshThrough139PresentationSpec
    {
        public const int LastDocumentedPushBatch = 139;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough136PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough130PresentationSpec.RefreshThroughBatch;

        public static int MinSpacingMinutes =>
            CampusGoPacketTailRefreshThrough136PresentationSpec.MinSpacingMinutes;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-139.md";
    }
}