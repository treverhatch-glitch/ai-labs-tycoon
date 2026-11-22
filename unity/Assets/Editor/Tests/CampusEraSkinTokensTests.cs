using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusEraSkinTokensTests
    {
        [Test]
        public void Transition_matches_polish_checklist()
        {
            Assert.AreEqual(0.8f, CampusEraSkinTokens.EraTransitionSeconds, 0.001f);
        }

        [Test]
        public void Documented_eras_non_empty()
        {
            Assert.GreaterOrEqual(CampusEraSkinTokens.DocumentedEraIds.Length, 4);
        }
    }
}