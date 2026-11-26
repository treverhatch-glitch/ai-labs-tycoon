namespace AiLabs.Unity.Hq
{
    /// <summary>Explainable HUD index refreshed after STATUS quad; rules use DisplayQueries only.</summary>
    public static class CampusExplainableHudDisplayQueriesTailPresentationSpec
    {
        public static int DocumentedBreakdownDrawers =>
            CampusExplainableHudIndexPresentationSpec.DocumentedBreakdownDrawers;

        public static int PostStatusSyncBatch =>
            CampusAlt76StatusSyncThrough187PresentationSpec.LastSyncedBatch;

        public const string TailDoc = "EXPLAINABLE-HUD-DISPLAY-QUERIES-TAIL.md";
    }
}