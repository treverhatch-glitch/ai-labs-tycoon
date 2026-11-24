using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough139PresentationSpecTests
    {
        [Test]
        public void Last_synced_139()
        {
            Assert.AreEqual(139, CampusAlt76StatusSyncThrough139PresentationSpec.LastSyncedBatch);
        }

        [Test]
        public void Go_151_manifest_136()
        {
            Assert.AreEqual(151, CampusAlt76StatusSyncThrough139PresentationSpec.GoPacketLastBatch);
            Assert.AreEqual(136, CampusAlt76StatusSyncThrough139PresentationSpec.ManifestRefreshBatch);
        }
    }
}