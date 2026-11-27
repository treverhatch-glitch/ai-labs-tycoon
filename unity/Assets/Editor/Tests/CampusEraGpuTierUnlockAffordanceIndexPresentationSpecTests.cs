using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusEraGpuTierUnlockAffordanceIndexPresentationSpecTests
    {
        [Test]
        public void Schema_slot_matches_era_gate_151()
        {
            Assert.AreEqual(151, CampusEraGpuTierUnlockAffordanceIndexPresentationSpec.EraSchemaSlot);
        }
    }
}