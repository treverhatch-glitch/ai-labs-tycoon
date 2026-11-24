namespace AiLabs.Unity.Hq
{
    /// <summary>GO packet tail through batch 151 (see GO-PACKET-TAIL-REFRESH-151.md).</summary>
    public static class CampusGoPacketTailRefreshThrough151PresentationSpec
    {
        public const int LastDocumentedPushBatch = 151;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough148PresentationSpec.LastDocumentedPushBatch;

        public static int ManifestRefreshBatch =>
            CampusIndexLaneManifestTailRefreshThrough136PresentationSpec.RefreshThroughBatch;

        public static int MinSpacingMinutes =>
            CampusGoPacketTailRefreshThrough148PresentationSpec.MinSpacingMinutes;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-151.md";
    }
}