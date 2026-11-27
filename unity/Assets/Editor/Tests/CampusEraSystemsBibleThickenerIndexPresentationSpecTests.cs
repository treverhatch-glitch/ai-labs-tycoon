using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusEraSystemsBibleThickenerIndexPresentationSpecTests
    {
        [Test]
        public void Era_schema_span_covers_149_through_153()
        {
            Assert.LessOrEqual(149, CampusEraSystemsBibleThickenerIndexPresentationSpec.FirstEraSchemaSlot);
            Assert.GreaterOrEqual(153, CampusEraSystemsBibleThickenerIndexPresentationSpec.LastEraSchemaSlot);
        }
    }
}