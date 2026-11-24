using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough166PresentationSpecTests
    {
        [Test]
        public void Go_166_links()
        {
            Assert.AreEqual(166, CampusGoPacketTailRefreshThrough166PresentationSpec.LastDocumentedPushBatch);
            Assert.AreEqual(145, CampusGoPacketTailRefreshThrough166PresentationSpec.StatusSyncLastBatch);
        }
    }
}