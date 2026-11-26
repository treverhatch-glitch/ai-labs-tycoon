using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough196PresentationSpecTests
    {
        [Test]
        public void Last_synced_advances_from_193()
        {
            Assert.Greater(
                CampusAlt76StatusSyncThrough196PresentationSpec.LastSyncedBatch,
                CampusAlt76StatusSyncThrough193PresentationSpec.LastSyncedBatch);
        }
    }
}