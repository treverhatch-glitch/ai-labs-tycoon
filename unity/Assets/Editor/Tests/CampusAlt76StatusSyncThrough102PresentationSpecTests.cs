using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough102PresentationSpecTests
    {
        [Test]
        public void Last_synced_102()
        {
            Assert.AreEqual(102, CampusAlt76StatusSyncThrough102PresentationSpec.LastSyncedBatch);
        }

        [Test]
        public void Ahead_of_98_sync()
        {
            Assert.Greater(
                CampusAlt76StatusSyncThrough102PresentationSpec.LastSyncedBatch,
                CampusAlt76StatusSyncThrough102PresentationSpec.PriorSyncLastBatch);
        }

        [Test]
        public void Go_packet_102()
        {
            Assert.AreEqual(102, CampusAlt76StatusSyncThrough102PresentationSpec.GoPacketLastBatch);
        }
    }
}