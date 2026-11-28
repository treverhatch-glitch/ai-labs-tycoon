namespace AiLabs.Unity.Hq
{
    /// <summary>GO packet tail through batch 133 (see GO-PACKET-TAIL-REFRESH-133.md).</summary>
    public static class CampusGoPacketTailRefreshThrough133PresentationSpec
    {
        public const int LastDocumentedPushBatch = 133;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough130PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough127PresentationSpec.RefreshThroughBatch;

        public static int MinSpacingMinutes =>
            CampusGoPacketTailRefreshThrough130PresentationSpec.MinSpacingMinutes;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-133.md";
    }
}