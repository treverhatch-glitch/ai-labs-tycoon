namespace AiLabs.Unity.Hq
{
    /// <summary>GPU tier unlock affordance index (era schema 151); ties staff/GPU panel chrome.</summary>
    public static class CampusEraGpuTierUnlockAffordanceIndexPresentationSpec
    {
        public const int EraSchemaSlot = 151;

        public static int StaffGpuLayers =>
            CampusStaffGpuPanelIndexPresentationSpec.DocumentedStaffGpuLayers;

        public const string IndexDoc = "ERA-GPU-TIER-UNLOCK-AFFORDANCE-INDEX.md";
    }
}