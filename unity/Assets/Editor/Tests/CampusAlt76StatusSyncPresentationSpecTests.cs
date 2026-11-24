using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncPresentationSpecTests
    {
        [Test]
        public void Last_synced_batch_ahead_of_prior_refresh()
        {
            Assert.Greater(
                CampusAlt76StatusSyncPresentationSpec.LastSyncedBatch,
                CampusAlt76StatusSyncPresentationSpec.PriorRefreshLastBatch);
        }

        [Test]
        public void Extended_lane_entries_positive()
        {
            Assert.Greater(CampusAlt76StatusSyncPresentationSpec.ExtendedLaneEntryCount, 0);
        }
    }
}