namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76StatusSyncThrough196PresentationSpec
    {
        public const int LastSyncedBatch = 196;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough193PresentationSpec.LastSyncedBatch;

        public static int RulesAdjacentLastBatch =>
            CampusRulesAdjacentGraphicsTailRefreshThrough280PresentationSpec
                .TailRefreshThroughPresentationBatch;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-196.md";
    }
}