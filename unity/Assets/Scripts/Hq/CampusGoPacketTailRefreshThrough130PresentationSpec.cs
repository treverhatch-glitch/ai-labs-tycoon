namespace AiLabs.Unity.Hq
{
    /// <summary>GO packet tail through batch 130 (see GO-PACKET-TAIL-REFRESH-130.md).</summary>
    public static class CampusGoPacketTailRefreshThrough130PresentationSpec
    {
        public const int LastDocumentedPushBatch = 130;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough127PresentationSpec.LastDocumentedPushBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough127PresentationSpec.LastSyncedBatch;

        public static int MinSpacingMinutes =>
            CampusGoPacketTailRefreshThrough127PresentationSpec.MinSpacingMinutes;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-130.md";
    }
}