using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusStudioCiGraphicsAdjacentPresentationSpecTests
    {
        [Test]
        public void Studio_ci_rows()
        {
            Assert.AreEqual(2, CampusStudioCiGraphicsAdjacentPresentationSpec.DocumentedStudioCiRows);
        }
    }
}