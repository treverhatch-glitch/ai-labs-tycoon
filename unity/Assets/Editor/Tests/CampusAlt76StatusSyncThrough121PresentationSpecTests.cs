using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough121PresentationSpecTests
    {
        [Test]
        public void Last_synced_121()
        {
            Assert.AreEqual(121, CampusAlt76StatusSyncThrough121PresentationSpec.LastSyncedBatch);
        }

        [Test]
        public void Prior_118()
        {
            Assert.AreEqual(118, CampusAlt76StatusSyncThrough121PresentationSpec.PriorSyncLastBatch);
        }

        [Test]
        public void Go_and_tail_align()
        {
            Assert.AreEqual(121, CampusAlt76StatusSyncThrough121PresentationSpec.GoPacketLastBatch);
            Assert.AreEqual(118, CampusAlt76StatusSyncThrough121PresentationSpec.TailRollupRefreshBatch);
        }
    }
}