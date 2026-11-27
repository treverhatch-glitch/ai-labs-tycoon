using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusRulesAdjacentEraSystemsTailIndexPresentationSpecTests
    {
        [Test]
        public void Era_schema_tail_ends_at_153()
        {
            Assert.AreEqual(153,
                CampusRulesAdjacentEraSystemsTailIndexPresentationSpec.LastEraSchemaSlot);
        }
    }
}