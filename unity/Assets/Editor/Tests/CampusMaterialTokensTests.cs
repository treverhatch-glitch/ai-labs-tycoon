using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusMaterialTokensTests
    {
        [Test]
        public void Floor_smoother_than_props()
        {
            Assert.Less(CampusMaterialTokens.FloorSmoothness, CampusMaterialTokens.DefaultSmoothness + 0.01f);
        }

        [Test]
        public void Gpu_blade_more_reflective()
        {
            Assert.Greater(CampusMaterialTokens.GpuBladeSmoothness, CampusMaterialTokens.DefaultSmoothness);
        }
    }
}