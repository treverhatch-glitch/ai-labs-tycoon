using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough130PresentationSpecTests
    {
        [Test]
        public void Last_synced_130()
        {
            Assert.AreEqual(130, CampusAlt76StatusSyncThrough130PresentationSpec.LastSyncedBatch);
        }

        [Test]
        public void Go_133_manifest_127()
        {
            Assert.AreEqual(133, CampusAlt76StatusSyncThrough130PresentationSpec.GoPacketLastBatch);
            Assert.AreEqual(127, CampusAlt76StatusSyncThrough130PresentationSpec.ManifestRefreshBatch);
        }
    }
}