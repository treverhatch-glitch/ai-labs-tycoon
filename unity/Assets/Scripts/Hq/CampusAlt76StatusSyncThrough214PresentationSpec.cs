namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76StatusSyncThrough214PresentationSpec
    {
        public const int LastSyncedBatch = 214;

        public static int PriorSyncLastBatch =>
            CampusAlt76StatusSyncThrough211PresentationSpec.LastSyncedBatch;

        public static int RulesAdjacentEraRows =>
            CampusRulesAdjacentEraSystemsTailIndexPresentationSpec.DocumentedEraPresentationRows;

        public const string SyncDoc = "ALT-76-STATUS-SYNC-214.md";
    }
}