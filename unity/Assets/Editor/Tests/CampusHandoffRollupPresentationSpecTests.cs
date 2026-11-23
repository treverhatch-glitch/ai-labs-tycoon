using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusHandoffRollupPresentationSpecTests
    {
        [Test]
        public void Handoff_kind_count()
        {
            Assert.AreEqual(5, CampusHandoffRollupPresentationSpec.DocumentedHandoffKinds);
        }

        [Test]
        public void Expected_counts_match_prepush_gate()
        {
            Assert.AreEqual(30, CampusHandoffRollupPresentationSpec.ExpectedPropCount);
            Assert.AreEqual(10, CampusHandoffRollupPresentationSpec.ExpectedIconCount);
            Assert.AreEqual(4, CampusHandoffRollupPresentationSpec.ExpectedSilhouetteCount);
        }
    }
}