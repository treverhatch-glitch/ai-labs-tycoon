namespace AiLabs.Unity.Hq
{
    /// <summary>GO packet tail through batch 124 (see GO-PACKET-TAIL-REFRESH-124.md).</summary>
    public static class CampusGoPacketTailRefreshThrough124PresentationSpec
    {
        public const int LastDocumentedPushBatch = 124;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough121PresentationSpec.LastDocumentedPushBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough121PresentationSpec.LastSyncedBatch;

        public static int MinSpacingMinutes =>
            CampusGoPacketTailRefreshThrough121PresentationSpec.MinSpacingMinutes;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-124.md";
    }
}