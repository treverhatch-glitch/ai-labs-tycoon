using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusAlt76StatusSyncThrough166PresentationSpecTests
    {
        [Test]
        public void Status_166()
        {
            Assert.AreEqual(166, CampusAlt76StatusSyncThrough166PresentationSpec.LastSyncedBatch);
        }
    }
}