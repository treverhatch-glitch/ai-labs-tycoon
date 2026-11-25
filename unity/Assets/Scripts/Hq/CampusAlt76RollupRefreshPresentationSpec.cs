namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 rollup refresh (see ALT-76-ROLLUP-REFRESH.md).</summary>
    public static class CampusAlt76RollupRefreshPresentationSpec
    {
        public const int LastDocumentedBatch = 75;

        public static int PriorRollupLastBatch => CampusAlt76RollupPresentationSpec.LastRemoteDocumentedBatch;

        public static int Post60ManifestEntries => CampusIndexLaneManifestPresentationSpec.Post60IndexEntryCount;

        public const string RollupRefreshDoc = "ALT-76-ROLLUP-REFRESH.md";
    }
}