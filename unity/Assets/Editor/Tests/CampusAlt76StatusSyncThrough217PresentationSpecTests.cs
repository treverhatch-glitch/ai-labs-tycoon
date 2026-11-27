using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough217PresentationSpecTests
    {
        [Test]
        public void Last_synced_advances_from_214()
        {
            Assert.Greater(
                CampusAlt76StatusSyncThrough217PresentationSpec.LastSyncedBatch,
                CampusAlt76StatusSyncThrough214PresentationSpec.LastSyncedBatch);
        }
    }
}