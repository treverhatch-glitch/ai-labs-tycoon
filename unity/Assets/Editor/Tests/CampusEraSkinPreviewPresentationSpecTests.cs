using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusEraSkinPreviewPresentationSpecTests
    {
        [Test]
        public void Preview_knows_era_json_path()
        {
            Assert.IsTrue(CampusEraSkinPreviewPresentationSpec.EraSkinsJson.Contains("era-skin-tokens.json"));
        }
    }
}