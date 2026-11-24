using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough98PresentationSpecTests
    {
        [Test]
        public void Last_synced_batch_98()
        {
            Assert.AreEqual(98, CampusAlt76StatusSyncThrough98PresentationSpec.LastSyncedBatch);
        }

        [Test]
        public void Ahead_of_89_sync()
        {
            Assert.Greater(
                CampusAlt76StatusSyncThrough98PresentationSpec.LastSyncedBatch,
                CampusAlt76StatusSyncThrough98PresentationSpec.PriorSyncLastBatch);
        }

        [Test]
        public void Go_packet_aligns_98()
        {
            Assert.AreEqual(98, CampusAlt76StatusSyncThrough98PresentationSpec.GoPacketLastBatch);
        }
    }
}