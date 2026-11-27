namespace AiLabs.Unity.Hq
{
    /// <summary>Rolls up era presentation chrome rows 322–326 for bible/systems navigation.</summary>
    public static class CampusEraSystemsBibleThickenerIndexPresentationSpec
    {
        public const int DocumentedEraChromeSpecs = 5;

        public static int FirstEraSchemaSlot =>
            CampusEraReservedComputeHudChromePresentationSpec.EraSchemaSlot;

        public static int LastEraSchemaSlot =>
            CampusEraShadyFreezeAuditHudChromePresentationSpec.EraSchemaSlot;

        public static int HudTailRefreshBatch =>
            CampusExplainableHudTailRefreshThrough315PresentationSpec
                .TailRefreshThroughPresentationBatch;

        public const string IndexDoc = "ERA-SYSTEMS-BIBLE-THICKENER-INDEX.md";
    }
}