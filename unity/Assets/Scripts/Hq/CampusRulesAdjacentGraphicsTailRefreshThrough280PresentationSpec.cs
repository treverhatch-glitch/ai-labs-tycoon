namespace AiLabs.Unity.Hq
{
    /// <summary>Rules-adjacent tail refresh after local presentation batch 280.</summary>
    public static class CampusRulesAdjacentGraphicsTailRefreshThrough280PresentationSpec
    {
        public const int TailRefreshThroughPresentationBatch = 280;

        public static int RulesAdjacentRows =>
            CampusRulesAdjacentGraphicsTailIndexPresentationSpec.DocumentedRulesAdjacentRows;

        public static int RollupRefreshLastBatch =>
            CampusAlt76RollupRefreshB193PresentationSpec.LastDocumentedBatch;

        public const string RefreshDoc = "RULES-ADJACENT-GRAPHICS-TAIL-REFRESH-280.md";
    }
}