namespace AiLabs.Unity.Hq
{
    /// <summary>GO packet refresh for tail batches (see GO-PACKET-TAIL-REFRESH.md).</summary>
    public static class CampusGoPacketTailRefreshPresentationSpec
    {
        public static int ExtendedLaneLastBatch =>
            CampusIndexLaneManifestTailPresentationSpec.ExtendedLaneBatchEnd;

        public static int LastStatusSyncedBatch =>
            CampusAlt76StatusSyncPresentationSpec.LastSyncedBatch;

        public static int MinSpacingMinutes =>
            CampusGoPacketIndexLaneRefreshPresentationSpec.MinSpacingMinutes;

        public const string TailRefreshDoc = "GO-PACKET-TAIL-REFRESH.md";
        public const string IndexLaneRefreshDoc = "GO-PACKET-INDEX-LANE-REFRESH.md";
    }
}