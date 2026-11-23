using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusWorldMapHostIndexPresentationSpecTests
    {
        [Test]
        public void Layer_count()
        {
            Assert.AreEqual(2, CampusWorldMapHostIndexPresentationSpec.DocumentedWorldMapLayers);
        }

        [Test]
        public void Bloc_count_matches_host_spec()
        {
            Assert.AreEqual(
                CampusWorldMapHostPresentationSpec.KnownBlocCount,
                CampusWorldMapHostIndexPresentationSpec.KnownBlocCount);
        }
    }
}