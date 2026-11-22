using AiLabs.Unity.Hq;
using NUnit.Framework;
using UnityEngine;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class RivalPresentationPaletteTests
    {
        [Test]
        public void Same_lab_id_same_accent()
        {
            Color a = RivalPresentationPalette.AccentFor("openaye");
            Color b = RivalPresentationPalette.AccentFor("openaye");
            Assert.AreEqual(a, b);
            Assert.Greater(a.a, 0f);
        }

        [Test]
        public void Curated_rivals_differ_from_hash_fallback()
        {
            Color open = RivalPresentationPalette.AccentFor("openaye");
            Color unknown = RivalPresentationPalette.AccentFor("unknown-lab-xyz");
            Assert.AreNotEqual(open, unknown);
        }
    }
}