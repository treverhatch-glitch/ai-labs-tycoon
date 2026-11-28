using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough199PresentationSpecTests
    {
        [Test]
        public void Last_synced_advances_from_196()
        {
            Assert.Greater(
                CampusAlt76StatusSyncThrough199PresentationSpec.LastSyncedBatch,
                CampusAlt76StatusSyncThrough196PresentationSpec.LastSyncedBatch);
        }
    }
}