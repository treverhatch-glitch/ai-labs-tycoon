using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough115PresentationSpecTests
    {
        [Test]
        public void Last_synced_115()
        {
            Assert.AreEqual(115, CampusAlt76StatusSyncThrough115PresentationSpec.LastSyncedBatch);
        }

        [Test]
        public void Prior_sync_109()
        {
            Assert.AreEqual(109, CampusAlt76StatusSyncThrough115PresentationSpec.PriorSyncLastBatch);
        }

        [Test]
        public void Go_and_manifest_align()
        {
            Assert.AreEqual(115, CampusAlt76StatusSyncThrough115PresentationSpec.GoPacketLastBatch);
            Assert.AreEqual(112, CampusAlt76StatusSyncThrough115PresentationSpec.ManifestRefreshBatch);
        }
    }
}