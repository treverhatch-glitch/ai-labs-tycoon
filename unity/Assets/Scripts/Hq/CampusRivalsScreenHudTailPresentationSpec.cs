namespace AiLabs.Unity.Hq
{
    /// <summary>Rivals screen HUD tail (layout 48; NEXT art table).</summary>
    public static class CampusRivalsScreenHudTailPresentationSpec
    {
        public static float LabListWidthFraction =>
            CampusRivalsScreenPresentationSpec.LabListWidthFraction;

        public static int PostRollupBatch =>
            CampusAlt76RollupRefreshB190PresentationSpec.LastDocumentedBatch;

        public const string TailDoc = "RIVALS-SCREEN-HUD-TAIL.md";
    }
}