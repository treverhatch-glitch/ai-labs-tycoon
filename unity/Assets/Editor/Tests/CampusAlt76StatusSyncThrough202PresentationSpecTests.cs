using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough202PresentationSpecTests
    {
        [Test]
        public void Last_synced_advances_from_199()
        {
            Assert.Greater(
                CampusAlt76StatusSyncThrough202PresentationSpec.LastSyncedBatch,
                CampusAlt76StatusSyncThrough199PresentationSpec.LastSyncedBatch);
        }
    }
}