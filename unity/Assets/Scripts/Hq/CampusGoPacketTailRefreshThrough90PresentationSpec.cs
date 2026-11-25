namespace AiLabs.Unity.Hq
{
    /// <summary>GO packet tail through batch 90 (see GO-PACKET-TAIL-REFRESH-90.md).</summary>
    public static class CampusGoPacketTailRefreshThrough90PresentationSpec
    {
        public const int LastDocumentedPushBatch = 90;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough88PresentationSpec.LastDocumentedPushBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough89PresentationSpec.LastSyncedBatch;

        public static int RollupLastBatch =>
            CampusAlt76RollupRefreshB89PresentationSpec.LastDocumentedBatch;

        public static int MinSpacingMinutes =>
            CampusGoPacketTailRefreshThrough88PresentationSpec.MinSpacingMinutes;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-90.md";
    }
}