using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough154PresentationSpecTests
    {
        [Test]
        public void Status_154()
        {
            Assert.AreEqual(154, CampusAlt76StatusSyncThrough154PresentationSpec.LastSyncedBatch);
        }
    }
}