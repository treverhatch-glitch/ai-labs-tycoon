namespace AiLabs.Unity.Hq
{
    public static class CampusGoPacketTailRefreshThrough238PresentationSpec
    {
        public const int LastDocumentedPushBatch = 238;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough235PresentationSpec.LastDocumentedPushBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough181PresentationSpec.LastSyncedBatch;

        public static int TailRollupRefreshBatch =>
            CampusPresentationTailRollupRefreshThrough178PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-238.md";
    }
}