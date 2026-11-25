namespace AiLabs.Unity.Hq
{
    /// <summary>GO packet tail through batch 127 (see GO-PACKET-TAIL-REFRESH-127.md).</summary>
    public static class CampusGoPacketTailRefreshThrough127PresentationSpec
    {
        public const int LastDocumentedPushBatch = 127;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough124PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough121PresentationSpec.RefreshThroughBatch;

        public static int MinSpacingMinutes =>
            CampusGoPacketTailRefreshThrough124PresentationSpec.MinSpacingMinutes;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-127.md";
    }
}