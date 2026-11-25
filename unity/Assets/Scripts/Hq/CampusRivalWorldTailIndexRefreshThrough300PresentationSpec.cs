namespace AiLabs.Unity.Hq
{
    public static class CampusRivalWorldTailIndexRefreshThrough300PresentationSpec
    {
        public const int TailRefreshThroughPresentationBatch = 300;

        public static int TailEntryCount =>
            CampusRivalWorldTailIndexPresentationSpec.TailEntryCount;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough202PresentationSpec.RefreshThroughBatch;

        public const string RefreshDoc = "RIVAL-WORLD-TAIL-INDEX-REFRESH-300.md";
    }
}