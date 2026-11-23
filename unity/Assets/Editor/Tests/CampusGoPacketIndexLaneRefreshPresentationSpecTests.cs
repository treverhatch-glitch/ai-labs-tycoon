using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketIndexLaneRefreshPresentationSpecTests
    {
        [Test]
        public void Post60_range_matches_manifest()
        {
            Assert.AreEqual(66, CampusGoPacketIndexLaneRefreshPresentationSpec.Post60FirstBatch);
            Assert.AreEqual(74, CampusGoPacketIndexLaneRefreshPresentationSpec.Post60LastBatch);
        }

        [Test]
        public void Spacing_matches_template()
        {
            Assert.AreEqual(
                CampusGoPacketTemplatePresentationSpec.MinSpacingMinutes,
                CampusGoPacketIndexLaneRefreshPresentationSpec.MinSpacingMinutes);
        }
    }
}