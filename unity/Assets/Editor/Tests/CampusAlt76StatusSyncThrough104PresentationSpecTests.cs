using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough104PresentationSpecTests
    {
        [Test]
        public void Last_synced_104()
        {
            Assert.AreEqual(104, CampusAlt76StatusSyncThrough104PresentationSpec.LastSyncedBatch);
        }

        [Test]
        public void Ahead_of_102()
        {
            Assert.Greater(
                CampusAlt76StatusSyncThrough104PresentationSpec.LastSyncedBatch,
                CampusAlt76StatusSyncThrough104PresentationSpec.PriorSyncLastBatch);
        }

        [Test]
        public void Go_packet_106()
        {
            Assert.AreEqual(106, CampusAlt76StatusSyncThrough104PresentationSpec.GoPacketLastBatch);
        }
    }
}