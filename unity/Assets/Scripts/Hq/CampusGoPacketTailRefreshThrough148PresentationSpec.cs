namespace AiLabs.Unity.Hq
{
    /// <summary>GO packet tail through batch 148 (see GO-PACKET-TAIL-REFRESH-148.md).</summary>
    public static class CampusGoPacketTailRefreshThrough148PresentationSpec
    {
        public const int LastDocumentedPushBatch = 148;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough145PresentationSpec.LastDocumentedPushBatch;

        public static int StatusSyncLastBatch =>
            CampusAlt76StatusSyncThrough136PresentationSpec.LastSyncedBatch;

        public static int TailRollupRefreshBatch =>
            CampusPresentationTailRollupRefreshThrough133PresentationSpec.RefreshThroughBatch;

        public static int MinSpacingMinutes =>
            CampusGoPacketTailRefreshThrough145PresentationSpec.MinSpacingMinutes;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-148.md";
    }
}