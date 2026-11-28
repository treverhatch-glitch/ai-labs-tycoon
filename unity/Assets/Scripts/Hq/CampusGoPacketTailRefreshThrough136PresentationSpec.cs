namespace AiLabs.Unity.Hq
{
    /// <summary>GO packet tail through batch 136 (see GO-PACKET-TAIL-REFRESH-136.md).</summary>
    public static class CampusGoPacketTailRefreshThrough136PresentationSpec
    {
        public const int LastDocumentedPushBatch = 136;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough133PresentationSpec.LastDocumentedPushBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough130PresentationSpec.LastSyncedBatch;

        public static int TailRollupRefreshBatch =>
            CampusPresentationTailRollupRefreshThrough127PresentationSpec.RefreshThroughBatch;

        public static int MinSpacingMinutes =>
            CampusGoPacketTailRefreshThrough133PresentationSpec.MinSpacingMinutes;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-136.md";
    }
}