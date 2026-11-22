using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPropLoaderTests
    {
        [Test]
        public void LogResolve_does_not_throw_for_known_id()
        {
            Assert.DoesNotThrow(() => CampusPropLoader.LogResolve(CampusPropIds.Desk));
        }
    }
}