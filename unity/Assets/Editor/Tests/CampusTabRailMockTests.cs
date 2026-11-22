using AiLabs.Unity.Hq;
using NUnit.Framework;
using UnityEngine;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusTabRailMockTests
    {
        [Test]
        public void Stub_builds_five_tabs()
        {
            var root = new GameObject("Root");
            var mock = root.AddComponent<CampusTabRailMock>();
            mock.BuildStub(root.transform);
            var rail = root.transform.Find("TabRail");
            Assert.NotNull(rail);
            Assert.AreEqual(CampusTabRailMock.TabIds.Length, rail.childCount);
            Object.DestroyImmediate(root);
        }
    }
}