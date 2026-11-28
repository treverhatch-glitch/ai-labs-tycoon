using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough145PresentationSpecTests
    {
        [Test]
        public void Status_145()
        {
            Assert.AreEqual(145, CampusAlt76StatusSyncThrough145PresentationSpec.LastSyncedBatch);
        }
    }
}