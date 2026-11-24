namespace AiLabs.Unity.Hq
{
    /// <summary>GO packet tail through batch 98 (see GO-PACKET-TAIL-REFRESH-98.md).</summary>
    public static class CampusGoPacketTailRefreshThrough98PresentationSpec
    {
        public const int LastDocumentedPushBatch = 98;

        public static int PriorDocumentedPushBatch =>
            CampusGoPacketTailRefreshThrough90PresentationSpec.LastDocumentedPushBatch;

        public static int HqCompletionTailBatch => 99;

        public static int MinSpacingMinutes =>
            CampusGoPacketTailRefreshThrough90PresentationSpec.MinSpacingMinutes;

        public const string RefreshDoc = "GO-PACKET-TAIL-REFRESH-98.md";
    }
}