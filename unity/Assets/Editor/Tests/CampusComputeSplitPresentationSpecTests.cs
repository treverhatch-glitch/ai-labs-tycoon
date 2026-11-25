using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusComputeSplitPresentationSpecTests
    {
        [Test]
        public void Compute_icon_resolves()
        {
            Assert.NotNull(CampusIconCatalog.PathFor(CampusComputeSplitPresentationSpec.ComputeIconId));
        }
    }
}