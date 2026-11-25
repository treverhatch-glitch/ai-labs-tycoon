using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough127PresentationSpecTests
    {
        [Test]
        public void Last_synced_127()
        {
            Assert.AreEqual(127, CampusAlt76StatusSyncThrough127PresentationSpec.LastSyncedBatch);
        }

        [Test]
        public void Prior_121()
        {
            Assert.AreEqual(121, CampusAlt76StatusSyncThrough127PresentationSpec.PriorSyncLastBatch);
        }

        [Test]
        public void Go_and_tail_align()
        {
            Assert.AreEqual(127, CampusAlt76StatusSyncThrough127PresentationSpec.GoPacketLastBatch);
            Assert.AreEqual(121, CampusAlt76StatusSyncThrough127PresentationSpec.TailRollupRefreshBatch);
        }
    }
}