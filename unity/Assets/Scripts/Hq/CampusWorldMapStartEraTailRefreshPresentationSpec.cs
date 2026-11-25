namespace AiLabs.Unity.Hq
{
    /// <summary>NEXT.md host UI — world map + start-era picker (items 12–13); presentation tokens only.</summary>
    public static class CampusWorldMapStartEraTailRefreshPresentationSpec
    {
        public const int DocumentedNextItemStart = 12;

        public static int WorldMapLayers =>
            CampusWorldMapHostIndexPresentationSpec.DocumentedWorldMapLayers;

        public static int PostRollupBatch =>
            CampusAlt76RollupRefreshB184PresentationSpec.LastDocumentedBatch;

        public const string TailRefreshDoc = "WORLD-MAP-START-ERA-TAIL-REFRESH.md";
    }
}