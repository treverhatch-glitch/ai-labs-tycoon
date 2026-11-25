using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough160PresentationSpecTests
    {
        [Test]
        public void Status_160()
        {
            Assert.AreEqual(160, CampusAlt76StatusSyncThrough160PresentationSpec.LastSyncedBatch);
        }
    }
}