namespace AiLabs.Unity.Hq
{
    /// <summary>Rival feeling meter chrome (see RIVAL-FEELING-PRESENTATION.md).</summary>
    public static class CampusRivalFeelingPresentationSpec
    {
        public const int FeelingMin = -1000;
        public const int FeelingMax = 1000;
        public const float HostileBandMax = 0.35f;
        public const float FriendlyBandMin = 0.65f;
        public const float MeterHeightPixels = 6f;

        public static float NormalizedFill(int feelingToPlayer)
        {
            if (feelingToPlayer < FeelingMin) feelingToPlayer = FeelingMin;
            if (feelingToPlayer > FeelingMax) feelingToPlayer = FeelingMax;
            return (feelingToPlayer + FeelingMax) / (FeelingMax - FeelingMin);
        }
    }
}