namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough160PresentationSpec
    {
        public const int LastDocumentedPushBatch = 160;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough157PresentationSpec.LastDocumentedPushBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough142PresentationSpec.LastSyncedBatch;

        public static int TailRollupRefreshBatch =>
            CampusPresentationTailRollupRefreshThrough139PresentationSpec.RefreshThroughBatch;

        public static int MinSpacingMinutes =>
            CampusGoPacketTailRefreshThrough157PresentationSpec.MinSpacingMinutes;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-160.md";
    }
}