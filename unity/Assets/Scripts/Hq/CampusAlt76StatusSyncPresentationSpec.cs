namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 STATUS sync through tails (see ALT-76-STATUS-SYNC.md).</summary>
    public static class CampusAlt76StatusSyncPresentationSpec
    {
        public const int LastSyncedBatch = 82;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshPresentationSpec.LastDocumentedBatch;

        public static int ExtendedLaneEntryCount =>
            CampusIndexLaneManifestTailPresentationSpec.ExtendedLaneEntryCount;

        public const string StatusDoc = "ALT-76-STATUS.md";
        public const string SyncDoc = "ALT-76-STATUS-SYNC.md";
    }
}