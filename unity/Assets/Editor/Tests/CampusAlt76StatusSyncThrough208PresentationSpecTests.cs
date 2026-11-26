using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough208PresentationSpecTests
    {
        [Test]
        public void Last_synced_advances_from_205()
        {
            Assert.Greater(
                CampusAlt76StatusSyncThrough208PresentationSpec.LastSyncedBatch,
                CampusAlt76StatusSyncThrough205PresentationSpec.LastSyncedBatch);
        }
    }
}