using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough118PresentationSpecTests
    {
        [Test]
        public void Last_synced_118()
        {
            Assert.AreEqual(118, CampusAlt76StatusSyncThrough118PresentationSpec.LastSyncedBatch);
        }

        [Test]
        public void Prior_115()
        {
            Assert.AreEqual(115, CampusAlt76StatusSyncThrough118PresentationSpec.PriorSyncLastBatch);
        }

        [Test]
        public void Go_and_tail_align()
        {
            Assert.AreEqual(118, CampusAlt76StatusSyncThrough118PresentationSpec.GoPacketLastBatch);
            Assert.AreEqual(115, CampusAlt76StatusSyncThrough118PresentationSpec.TailRollupRefreshBatch);
        }
    }
}