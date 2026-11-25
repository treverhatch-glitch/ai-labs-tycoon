using AiLabs.Rules.Config;
using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class HqVisualPaletteTests
    {
        [Test]
        public void Every_startable_era_has_a_floor_skin()
        {
            string[] eras =
            {
                "dial-up-days", "the-punchline", "it-sees", "it-writes",
                "the-chat-boom", "agents", "harnesses"
            };
            foreach (string era in eras)
            {
                HqVisualPalette.EraSkin skin = HqVisualPalette.EraSkinFor(era);
                Assert.Greater(skin.Floor.r, 0f);
                Assert.Greater(skin.Accent.r, 0f);
            }
        }

        [Test]
        public void Room_purpose_colours_are_distinct()
        {
            var research = HqVisualPalette.PurposeColor(RoomPurpose.Research);
            var engineering = HqVisualPalette.PurposeColor(RoomPurpose.Engineering);
            var compute = HqVisualPalette.PurposeColor(RoomPurpose.Compute);
            Assert.AreNotEqual(research, engineering);
            Assert.AreNotEqual(engineering, compute);
        }

        [Test]
        public void Glow_emission_scales_with_permille()
        {
            var baseColor = HqVisualPalette.PurposeColor(RoomPurpose.Safety);
            var dim = HqVisualPalette.GlowEmission(baseColor, 0);
            var bright = HqVisualPalette.GlowEmission(baseColor, 800);
            Assert.Greater(bright.maxColorComponent, dim.maxColorComponent);
        }
    }
}