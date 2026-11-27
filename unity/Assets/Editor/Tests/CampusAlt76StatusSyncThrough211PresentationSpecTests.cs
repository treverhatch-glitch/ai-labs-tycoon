using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough211PresentationSpecTests
    {
        [Test]
        public void Last_synced_advances_from_208()
        {
            Assert.Greater(
                CampusAlt76StatusSyncThrough211PresentationSpec.LastSyncedBatch,
                CampusAlt76StatusSyncThrough208PresentationSpec.LastSyncedBatch);
        }
    }
}