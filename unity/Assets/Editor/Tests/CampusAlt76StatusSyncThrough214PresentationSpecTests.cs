using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough214PresentationSpecTests
    {
        [Test]
        public void Last_synced_advances_from_211()
        {
            Assert.Greater(
                CampusAlt76StatusSyncThrough214PresentationSpec.LastSyncedBatch,
                CampusAlt76StatusSyncThrough211PresentationSpec.LastSyncedBatch);
        }
    }
}