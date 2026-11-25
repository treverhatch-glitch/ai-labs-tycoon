using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough109PresentationSpecTests
    {
        [Test]
        public void Last_synced_109()
        {
            Assert.AreEqual(109, CampusAlt76StatusSyncThrough109PresentationSpec.LastSyncedBatch);
        }

        [Test]
        public void Ahead_of_104()
        {
            Assert.Greater(
                CampusAlt76StatusSyncThrough109PresentationSpec.LastSyncedBatch,
                CampusAlt76StatusSyncThrough109PresentationSpec.PriorSyncLastBatch);
        }

        [Test]
        public void Go_packet_109()
        {
            Assert.AreEqual(109, CampusAlt76StatusSyncThrough109PresentationSpec.GoPacketLastBatch);
        }
    }
}