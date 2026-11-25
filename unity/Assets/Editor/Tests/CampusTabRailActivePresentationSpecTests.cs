using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusTabRailActivePresentationSpecTests
    {
        [Test]
        public void Plaque_prop_resolves()
        {
            Assert.NotNull(CampusPropCatalog.PathFor(CampusTabRailActivePresentationSpec.PlaquePropId));
        }
    }
}