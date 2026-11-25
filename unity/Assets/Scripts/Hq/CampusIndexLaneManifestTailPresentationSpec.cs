namespace AiLabs.Unity.Hq
{
    /// <summary>Index lane manifest tail after batch 74 (see INDEX-LANE-MANIFEST-TAIL.md).</summary>
    public static class CampusIndexLaneManifestTailPresentationSpec
    {
        public const int TailBatchStart = 75;
        public const int TailBatchEnd = 78;
        public const int TailEntryCount = 4;

        public static int BaseManifestEnd =>
            CampusIndexLaneManifestPresentationSpec.Post60IndexBatchEnd;

        public static int ExtendedLaneBatchStart =>
            CampusIndexLaneManifestPresentationSpec.Post60IndexBatchStart;

        public static int ExtendedLaneBatchEnd => TailBatchEnd;

        public static int ExtendedLaneEntryCount =>
            CampusIndexLaneManifestPresentationSpec.Post60IndexEntryCount + TailEntryCount;

        public static int IndexedHqBands =>
            CampusHqStageStackCompletionPresentationSpec.IndexedHqBands;

        public const string TailDoc = "INDEX-LANE-MANIFEST-TAIL.md";
        public const string BaseManifestDoc = "INDEX-LANE-MANIFEST.md";
    }
}