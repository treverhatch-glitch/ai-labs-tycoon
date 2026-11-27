using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusEraEndlessScenarioHudChromePresentationSpecTests
    {
        [Test]
        public void Schema_slot_matches_era_gate_152()
        {
            Assert.AreEqual(152, CampusEraEndlessScenarioHudChromePresentationSpec.EraSchemaSlot);
        }
    }
}