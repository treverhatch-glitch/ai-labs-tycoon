namespace AiLabs.Unity.Hq
{
    /// <summary>Research screen HUD tail (layout 47; NEXT art table).</summary>
    public static class CampusResearchScreenHudTailPresentationSpec
    {
        public static float TechTreeWidthFraction =>
            CampusResearchScreenPresentationSpec.TechTreeWidthFraction;

        public static int PostRivalsHudBatch =>
            CampusRivalsScreenHudTailPresentationSpec.PostRollupBatch;

        public const string TailDoc = "RESEARCH-SCREEN-HUD-TAIL.md";
    }
}