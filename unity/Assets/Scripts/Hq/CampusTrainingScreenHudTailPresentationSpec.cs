namespace AiLabs.Unity.Hq
{
    /// <summary>Training screen HUD tail (screen layout batch 49; NEXT art table).</summary>
    public static class CampusTrainingScreenHudTailPresentationSpec
    {
        public static float RunQueueWidthFraction =>
            CampusTrainingScreenPresentationSpec.RunQueueWidthFraction;

        public static int PostStatusBatch =>
            CampusAlt76StatusSyncThrough190PresentationSpec.LastSyncedBatch;

        public const string TailDoc = "TRAINING-SCREEN-HUD-TAIL.md";
    }
}