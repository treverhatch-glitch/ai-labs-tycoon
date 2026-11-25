using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough172PresentationSpecTests
    {
        [Test]
        public void Status_172()
        {
            Assert.AreEqual(172, CampusAlt76StatusSyncThrough172PresentationSpec.LastSyncedBatch);
        }
    }
}