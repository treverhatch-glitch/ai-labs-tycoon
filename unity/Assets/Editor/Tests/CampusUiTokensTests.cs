using AiLabs.Unity.Hq;
using NUnit.Framework;
using UnityEngine;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusUiTokensTests
    {
        [Test]
        public void Ui_tokens_parse_and_differ()
        {
            Assert.AreNotEqual(CampusUiTokens.UiInk, CampusUiTokens.UiPaper);
            Assert.AreNotEqual(CampusUiTokens.UiDanger, CampusUiTokens.UiSlate);
            Assert.Greater(CampusUiTokens.DefaultSmoothness, CampusUiTokens.FloorSmoothness);
        }

        [Test]
        public void Hex_helper_returns_opaque_colours()
        {
            Color c = CampusUiTokens.Hex("#5A8CF2");
            Assert.AreEqual(1f, c.a);
            Assert.Greater(c.b, c.r);
        }
    }
}