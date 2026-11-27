using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusEraReservedComputeHudChromePresentationSpecTests
    {
        [Test]
        public void Schema_slot_matches_era_gate_149()
        {
            Assert.AreEqual(149, CampusEraReservedComputeHudChromePresentationSpec.EraSchemaSlot);
        }
    }
}