namespace AiLabs.Unity.Hq
{
    public static class CampusAlt76StatusSyncThrough220PresentationSpec
    {
        public const int LastSyncedBatch = 220;

        public static int PriorSyncedBatch =>
            CampusAlt76StatusSyncThrough217PresentationSpec.LastSyncedBatch;

        public const string RefreshDoc = "ALT-76-STATUS-SYNC-220.md";
    }
}
