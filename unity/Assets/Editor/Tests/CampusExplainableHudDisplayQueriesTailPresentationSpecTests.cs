using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusExplainableHudDisplayQueriesTailPresentationSpecTests
    {
        [Test]
        public void Hud_drawers_positive()
        {
            Assert.Greater(CampusExplainableHudDisplayQueriesTailPresentationSpec.DocumentedBreakdownDrawers, 0);
        }
    }
}