namespace AiLabs.Unity.Hq
{
    /// <summary>Rules-adjacent tail refresh after batch 248 staff land (batch 252 LOCAL).</summary>
    public static class CampusRulesAdjacentGraphicsTailRefreshPresentationSpec
    {
        public const int TailRefreshAfterBatch = 248;

        public static int RulesAdjacentRows =>
            CampusRulesAdjacentGraphicsTailIndexPresentationSpec.DocumentedRulesAdjacentRows;

        public static int StaffPresentationBatch =>
            CampusStaffPresentationTailIndexPresentationSpec.DocumentedPresentationBatch;

        public const string RefreshDoc = "RULES-ADJACENT-GRAPHICS-TAIL-REFRESH.md";
    }
}