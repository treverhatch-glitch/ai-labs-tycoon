using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough193PresentationSpecTests
    {
        [Test]
        public void Last_synced_advances_from_190()
        {
            Assert.Greater(
                CampusAlt76StatusSyncThrough193PresentationSpec.LastSyncedBatch,
                CampusAlt76StatusSyncThrough190PresentationSpec.LastSyncedBatch);
        }
    }
}