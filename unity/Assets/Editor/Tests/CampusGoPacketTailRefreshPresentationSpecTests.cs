using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshPresentationSpecTests
    {
        [Test]
        public void Extended_lane_matches_manifest_tail()
        {
            Assert.AreEqual(82, CampusGoPacketTailRefreshPresentationSpec.ExtendedLaneLastBatch);
        }

        [Test]
        public void Status_sync_matches_alt76()
        {
            Assert.AreEqual(
                CampusAlt76StatusSyncPresentationSpec.LastSyncedBatch,
                CampusGoPacketTailRefreshPresentationSpec.LastStatusSyncedBatch);
        }

        [Test]
        public void Spacing_matches_template()
        {
            Assert.AreEqual(
                CampusGoPacketTemplatePresentationSpec.MinSpacingMinutes,
                CampusGoPacketTailRefreshPresentationSpec.MinSpacingMinutes);
        }
    }
}