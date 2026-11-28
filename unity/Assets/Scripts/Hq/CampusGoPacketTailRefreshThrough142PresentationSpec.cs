namespace AiLabs.Unity.Hq
{
    /// <summary>GO packet tail through batch 142 (see GO-PACKET-TAIL-REFRESH-142.md).</summary>
    public static class CampusGoPacketTailRefreshThrough142PresentationSpec
    {
        public const int LastDocumentedPushBatch = 142;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough139PresentationSpec.LastDocumentedPushBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough133PresentationSpec.LastSyncedBatch;

        public static int TailRollupRefreshBatch =>
            CampusPresentationTailRollupRefreshThrough130PresentationSpec.RefreshThroughBatch;

        public static int MinSpacingMinutes =>
            CampusGoPacketTailRefreshThrough139PresentationSpec.MinSpacingMinutes;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-142.md";
    }
}