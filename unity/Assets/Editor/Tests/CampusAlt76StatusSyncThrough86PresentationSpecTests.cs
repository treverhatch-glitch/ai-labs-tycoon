using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough86PresentationSpecTests
    {
        [Test]
        public void Last_synced_ahead_of_batch_83_sync()
        {
            Assert.Greater(
                CampusAlt76StatusSyncThrough86PresentationSpec.LastSyncedBatch,
                CampusAlt76StatusSyncThrough86PresentationSpec.PriorSyncLastBatch);
        }

        [Test]
        public void Matches_rollup_refresh_last_batch()
        {
            Assert.AreEqual(
                CampusAlt76RollupRefreshB85PresentationSpec.LastDocumentedBatch,
                CampusAlt76StatusSyncThrough86PresentationSpec.RollupRefreshLastBatch);
        }
    }
}