using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough136PresentationSpecTests
    {
        [Test]
        public void Last_synced_136()
        {
            Assert.AreEqual(136, CampusAlt76StatusSyncThrough136PresentationSpec.LastSyncedBatch);
        }

        [Test]
        public void Go_145_manifest_133()
        {
            Assert.AreEqual(145, CampusAlt76StatusSyncThrough136PresentationSpec.GoPacketLastBatch);
            Assert.AreEqual(133, CampusAlt76StatusSyncThrough136PresentationSpec.ManifestRefreshBatch);
        }
    }
}