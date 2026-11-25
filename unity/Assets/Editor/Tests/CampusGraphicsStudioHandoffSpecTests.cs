using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusGraphicsStudioHandoffSpecTests
    {
        [Test]
        public void Graphics_branch_name_is_stable()
        {
            Assert.IsTrue(CampusGraphicsStudioHandoffSpec.GraphicsBranch.Contains("graphics-pipeline"));
        }
    }
}