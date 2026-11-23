namespace AiLabs.Unity.Hq
{
    /// <summary>Graphics GO packet conventions (see GO-PACKET-TEMPLATE.md).</summary>
    public static class CampusGoPacketTemplatePresentationSpec
    {
        public const int MinSpacingMinutes = 30;

        public const int IndexLaneFirstBatch = CampusPresentationIndexHubPresentationSpec.FirstHubBatch;

        public const string StorePacketDir = "internal/graphics-batch";
    }
}