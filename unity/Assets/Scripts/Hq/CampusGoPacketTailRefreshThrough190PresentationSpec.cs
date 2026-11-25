namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough190PresentationSpec
    {
        public const int LastDocumentedPushBatch = 190;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough187PresentationSpec.LastDocumentedPushBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough157PresentationSpec.LastSyncedBatch;

        public static int TailRollupRefreshBatch =>
            CampusPresentationTailRollupRefreshThrough154PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-190.md";
    }
}