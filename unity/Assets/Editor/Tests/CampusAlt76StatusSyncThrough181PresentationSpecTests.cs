using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough181PresentationSpecTests
    {
        [Test]
        public void Status_181()
        {
            Assert.AreEqual(181, CampusAlt76StatusSyncThrough181PresentationSpec.LastSyncedBatch);
        }
    }
}