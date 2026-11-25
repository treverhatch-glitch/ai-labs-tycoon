using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough89PresentationSpecTests
    {
        [Test]
        public void Last_synced_ahead_of_86_sync()
        {
            Assert.Greater(
                CampusAlt76StatusSyncThrough89PresentationSpec.LastSyncedBatch,
                CampusAlt76StatusSyncThrough89PresentationSpec.PriorSyncLastBatch);
        }

        [Test]
        public void Capture_tail_entries_four()
        {
            Assert.AreEqual(4, CampusAlt76StatusSyncThrough89PresentationSpec.CaptureTailEntries);
        }
    }
}