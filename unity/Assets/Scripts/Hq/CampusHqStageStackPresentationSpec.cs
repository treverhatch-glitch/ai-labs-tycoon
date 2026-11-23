namespace AiLabs.Unity.Hq
{
    /// <summary>HQ stage band index (see HQ-STAGE-STACK.md).</summary>
    public static class CampusHqStageStackPresentationSpec
    {
        public const int HqPresentationBandCount = 5;

        public static float LotViewportWidthFraction =>
            CampusHqLotViewportPresentationSpec.ViewportWidthFraction;
    }
}