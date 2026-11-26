using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusStaffGpuEraSystemsBibleTailPresentationSpecTests
    {
        [Test]
        public void Era_schema_matches_gpu_tier_slot()
        {
            Assert.AreEqual(151, CampusStaffGpuEraSystemsBibleTailPresentationSpec.EraGpuTierSchema);
        }
    }
}