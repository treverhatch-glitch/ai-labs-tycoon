using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusSilhouetteCatalogTests
    {
        [Test]
        public void Supported_silhouettes_match_manifest()
        {
            Assert.AreEqual(4, CampusSilhouetteCatalog.Supported.Length);
            foreach (string id in CampusSilhouetteCatalog.Supported)
            {
                StringAssert.Contains("silhouette-" + id, CampusSilhouetteCatalog.PathFor(id));
                StringAssert.StartsWith("Assets/Art/Generated/rival-silhouettes/", CampusSilhouetteCatalog.PathFor(id));
            }
        }
    }
}