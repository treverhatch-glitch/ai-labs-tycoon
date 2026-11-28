using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusHudStyleGuidePipelineTailIndexPresentationSpecTests
    {
        [Test]
        public void Pipeline_rows()
        {
            Assert.AreEqual(
                4,
                CampusHudStyleGuidePipelineTailIndexPresentationSpec.DocumentedHudPipelineRows);
        }
    }
}