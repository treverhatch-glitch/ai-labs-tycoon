using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusRulesAdjacentGraphicsTailIndexPresentationSpecTests
    {
        [Test]
        public void Rules_adjacent_rows()
        {
            Assert.AreEqual(
                3,
                CampusRulesAdjacentGraphicsTailIndexPresentationSpec.DocumentedRulesAdjacentRows);
        }
    }
}