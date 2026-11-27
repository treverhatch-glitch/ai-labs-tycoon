using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough220PresentationSpecTests
    {
        [Test]
        public void Batch_354_constant_matches_lane()
        {
            Assert.AreEqual(220, CampusAlt76StatusSyncThrough220PresentationSpec.LastSyncedBatch);
        }
    }
}
