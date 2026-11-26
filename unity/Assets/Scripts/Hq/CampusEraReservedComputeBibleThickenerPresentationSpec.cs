namespace AiLabs.Unity.Hq
{
    /// <summary>Thickener linking reserved-compute HUD chrome to era bible index (schema 149).</summary>
    public static class CampusEraReservedComputeBibleThickenerPresentationSpec
    {
        public static int EraSchemaSlot =>
            CampusEraReservedComputeHudChromePresentationSpec.EraSchemaSlot;

        public static int EraBibleRows =>
            CampusEraSystemsBibleThickenerIndexPresentationSpec.DocumentedEraChromeSpecs;

        public const string ThickenerDoc = "ERA-RESERVED-COMPUTE-BIBLE-THICKENER.md";
    }
}