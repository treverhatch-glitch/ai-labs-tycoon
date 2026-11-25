using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusWorldMapStartEraTailRefreshPresentationSpecTests
    {
        [Test]
        public void Next_item_12()
        {
            Assert.AreEqual(12, CampusWorldMapStartEraTailRefreshPresentationSpec.DocumentedNextItemStart);
        }
    }
}