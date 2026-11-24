using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough133PresentationSpecTests
    {
        [Test]
        public void Last_synced_133()
        {
            Assert.AreEqual(133, CampusAlt76StatusSyncThrough133PresentationSpec.LastSyncedBatch);
        }

        [Test]
        public void Go_139_manifest_130()
        {
            Assert.AreEqual(139, CampusAlt76StatusSyncThrough133PresentationSpec.GoPacketLastBatch);
            Assert.AreEqual(130, CampusAlt76StatusSyncThrough133PresentationSpec.ManifestRefreshBatch);
        }
    }
}