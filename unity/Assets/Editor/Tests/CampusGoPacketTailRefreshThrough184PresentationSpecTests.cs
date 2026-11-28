using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGoPacketTailRefreshThrough184PresentationSpecTests
    {
        [Test]
        public void Go_184_links()
        {
            Assert.AreEqual(184, CampusGoPacketTailRefreshThrough184PresentationSpec.LastDocumentedPushBatch);
            Assert.AreEqual(154, CampusGoPacketTailRefreshThrough184PresentationSpec.StatusSyncLastBatch);
        }
    }
}