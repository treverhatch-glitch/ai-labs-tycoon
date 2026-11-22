using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPropIdsTests
    {
        [Test]
        public void Core_prop_ids_are_non_empty()
        {
            Assert.IsNotEmpty(CampusPropIds.BuildingModule);
            Assert.IsNotEmpty(CampusPropIds.ServerRack);
            Assert.IsNotEmpty(CampusPropIds.WorldPin);
            Assert.AreNotEqual(CampusPropIds.Desk, CampusPropIds.Chair);
        }
    }
}