namespace AiLabs.Unity.Hq
{
    /// <summary>ALT-76 rollup refresh through batch 85 (see ALT-76-ROLLUP-REFRESH-85.md).</summary>
    public static class CampusAlt76RollupRefreshB85PresentationSpec
    {
        public const int LastDocumentedBatch = 85;

        public static int PriorRefreshLastBatch =>
            CampusAlt76RollupRefreshPresentationSpec.LastDocumentedBatch;

        public static int HubTailRows =>
            CampusPresentationIndexHubTailPresentationSpec.TailEntryCount;

        public const string RefreshDoc = "ALT-76-ROLLUP-REFRESH-85.md";
    }
}