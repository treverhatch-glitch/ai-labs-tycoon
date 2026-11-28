using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusHqCashDisplayPresentationSpecTests
    {
        [Test]
        public void Hq_icon_resolves()
        {
            Assert.NotNull(CampusIconCatalog.PathFor(CampusHqCashDisplayPresentationSpec.HeaderIconId));
        }
    }
}