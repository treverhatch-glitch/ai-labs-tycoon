using AiLabs.Unity.Hq;
using NUnit.Framework;
namespace AiLabs.Unity.Editor.Tests {
    public sealed class CampusAlt76StatusSyncThrough223PresentationSpecTests {
        [Test]
        public void Batch_363_lane_constant() {
            Assert.AreEqual(223, CampusAlt76StatusSyncThrough223PresentationSpec.LastSyncedBatch);
        }
    }
}
