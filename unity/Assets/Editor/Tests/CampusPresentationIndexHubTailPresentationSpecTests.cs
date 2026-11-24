using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationIndexHubTailPresentationSpecTests
    {
        [Test]
        public void Hub_entries_eleven()
        {
            Assert.AreEqual(11, CampusPresentationIndexHubTailPresentationSpec.HubEntryCount);
        }

        [Test]
        public void Tail_rows_four()
        {
            Assert.AreEqual(4, CampusPresentationIndexHubTailPresentationSpec.TailEntryCount);
        }

        [Test]
        public void Extended_lane_positive()
        {
            Assert.Greater(CampusPresentationIndexHubTailPresentationSpec.ExtendedLaneEntryCount, 0);
        }
    }
}