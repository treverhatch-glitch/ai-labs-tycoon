using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough142PresentationSpecTests
    {
        [Test]
        public void Status_142()
        {
            Assert.AreEqual(142, CampusAlt76StatusSyncThrough142PresentationSpec.LastSyncedBatch);
            Assert.AreEqual(157, CampusAlt76StatusSyncThrough142PresentationSpec.GoPacketLastBatch);
        }
    }
}