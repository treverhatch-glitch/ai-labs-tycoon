using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusResearchScreenHudTailPresentationSpecTests
    {
        [Test]
        public void Tech_tree_fraction_positive()
        {
            Assert.Greater(CampusResearchScreenHudTailPresentationSpec.TechTreeWidthFraction, 0f);
        }
    }
}