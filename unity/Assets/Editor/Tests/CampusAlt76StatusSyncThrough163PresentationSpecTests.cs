using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough163PresentationSpecTests
    {
        [Test]
        public void Status_163()
        {
            Assert.AreEqual(163, CampusAlt76StatusSyncThrough163PresentationSpec.LastSyncedBatch);
        }
    }
}