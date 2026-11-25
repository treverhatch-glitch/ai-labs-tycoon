using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough175PresentationSpecTests
    {
        [Test]
        public void Status_175()
        {
            Assert.AreEqual(175, CampusAlt76StatusSyncThrough175PresentationSpec.LastSyncedBatch);
        }
    }
}