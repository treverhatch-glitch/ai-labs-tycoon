namespace AiLabs.Unity.Hq
{
    /// <summary>GO packet tail through batch 88 (see GO-PACKET-TAIL-REFRESH-88.md).</summary>
    public static class CampusGoPacketTailRefreshThrough88PresentationSpec
    {
        public const int LastDocumentedPushBatch = 88;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough86PresentationSpec.LastSyncedBatch;

        public static int PriorTailLaneLastBatch =>
            CampusGoPacketTailRefreshPresentationSpec.ExtendedLaneLastBatch;

        public static int MinSpacingMinutes =>
            CampusGoPacketTailRefreshPresentationSpec.MinSpacingMinutes;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-88.md";
    }
}