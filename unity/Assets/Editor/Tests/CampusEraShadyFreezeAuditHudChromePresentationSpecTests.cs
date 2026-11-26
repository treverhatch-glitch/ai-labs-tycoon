using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusEraShadyFreezeAuditHudChromePresentationSpecTests
    {
        [Test]
        public void Schema_slot_matches_era_gate_153()
        {
            Assert.AreEqual(153, CampusEraShadyFreezeAuditHudChromePresentationSpec.EraSchemaSlot);
        }
    }
}