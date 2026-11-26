using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough190PresentationSpecTests
    {
        [Test]
        public void Status_190()
        {
            Assert.AreEqual(190, CampusAlt76StatusSyncThrough190PresentationSpec.LastSyncedBatch);
        }
    }
}