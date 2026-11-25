using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough157PresentationSpecTests
    {
        [Test]
        public void Status_157()
        {
            Assert.AreEqual(157, CampusAlt76StatusSyncThrough157PresentationSpec.LastSyncedBatch);
        }
    }
}