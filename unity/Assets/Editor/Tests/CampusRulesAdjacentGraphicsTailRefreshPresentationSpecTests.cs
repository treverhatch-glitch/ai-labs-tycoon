using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusRulesAdjacentGraphicsTailRefreshPresentationSpecTests
    {
        [Test]
        public void Refresh_after_staff_248()
        {
            Assert.AreEqual(248, CampusRulesAdjacentGraphicsTailRefreshPresentationSpec.TailRefreshAfterBatch);
        }
    }
}