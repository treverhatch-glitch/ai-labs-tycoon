namespace AiLabs.Unity.Hq
{
    /// <summary>Bible hook: rules-adjacent era presentation row after schema 153 chrome.</summary>
    public static class CampusRulesAdjacentEraSystemsTailIndexPresentationSpec
    {
        public const int DocumentedEraPresentationRows = 6;

        public static int LastEraSchemaSlot =>
            CampusEraShadyFreezeAuditHudChromePresentationSpec.EraSchemaSlot;

        public static int RollupLastBatch =>
            CampusAlt76RollupRefreshB211PresentationSpec.LastDocumentedBatch;

        public const string TailDoc = "RULES-ADJACENT-ERA-SYSTEMS-TAIL.md";
    }
}