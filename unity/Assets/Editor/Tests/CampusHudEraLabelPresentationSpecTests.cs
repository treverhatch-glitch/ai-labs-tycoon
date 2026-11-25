using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusHudEraLabelPresentationSpecTests
    {
        [Test]
        public void Label_height_matches_date_row()
        {
            Assert.AreEqual(
                CampusHudDateWeekPresentationSpec.DateRowHeightPixels,
                CampusHudEraLabelPresentationSpec.LabelHeightPixels);
        }

        [Test]
        public void Transition_matches_era_skin_tokens()
        {
            Assert.AreEqual(
                CampusEraSkinTokens.EraTransitionSeconds,
                CampusHudEraLabelPresentationSpec.EraTransitionSeconds,
                0.001f);
        }
    }
}