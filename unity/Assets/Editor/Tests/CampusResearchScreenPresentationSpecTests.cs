using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusResearchScreenPresentationSpecTests
    {
        [Test]
        public void Research_layout_fractions()
        {
            Assert.AreEqual(0.62f, CampusResearchScreenPresentationSpec.TechTreeWidthFraction);
            Assert.AreEqual(0.38f, CampusResearchScreenPresentationSpec.DetailDrawerWidthFraction);
        }
    }
}