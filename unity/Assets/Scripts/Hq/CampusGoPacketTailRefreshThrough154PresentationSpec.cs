namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough154PresentationSpec
    {
        public const int LastDocumentedPushBatch = 154;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough151PresentationSpec.LastDocumentedPushBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough139PresentationSpec.LastSyncedBatch;

        public static int TailRollupRefreshBatch =>
            CampusPresentationTailRollupRefreshThrough136PresentationSpec.RefreshThroughBatch;

        public static int MinSpacingMinutes =>
            CampusGoPacketTailRefreshThrough151PresentationSpec.MinSpacingMinutes;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-154.md";
    }
}