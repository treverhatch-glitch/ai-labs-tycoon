namespace AiLabs.Unity.Hq
{
    /// <summary>Post-60 index lane manifest (see INDEX-LANE-MANIFEST.md).</summary>
    public static class CampusIndexLaneManifestPresentationSpec
    {
        public const int Post60IndexBatchStart = 66;
        public const int Post60IndexBatchEnd = 74;
        public const int Post60IndexEntryCount = 9;

        public static int HqCompletionIndexedBands =>
            CampusHqStageStackCompletionPresentationSpec.IndexedHqBands;

        public const string ManifestDoc = "INDEX-LANE-MANIFEST.md";
    }
}