using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusEraRivalIntentNewsTickerChromePresentationSpecTests
    {
        [Test]
        public void Schema_slot_matches_era_gate_150()
        {
            Assert.AreEqual(150, CampusEraRivalIntentNewsTickerChromePresentationSpec.EraSchemaSlot);
        }
    }
}