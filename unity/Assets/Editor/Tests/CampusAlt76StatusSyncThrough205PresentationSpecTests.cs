using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough205PresentationSpecTests
    {
        [Test]
        public void Last_synced_advances_from_202()
        {
            Assert.Greater(
                CampusAlt76StatusSyncThrough205PresentationSpec.LastSyncedBatch,
                CampusAlt76StatusSyncThrough202PresentationSpec.LastSyncedBatch);
        }
    }
}