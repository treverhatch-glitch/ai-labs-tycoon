using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTemplatePresentationSpecTests
    {
        [Test]
        public void Index_lane_starts_at_hub_first_batch()
        {
            Assert.AreEqual(50, CampusGoPacketTemplatePresentationSpec.IndexLaneFirstBatch);
        }

        [Test]
        public void Spacing_minutes()
        {
            Assert.AreEqual(30, CampusGoPacketTemplatePresentationSpec.MinSpacingMinutes);
        }
    }
}