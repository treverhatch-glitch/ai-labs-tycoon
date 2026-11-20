using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class LotTileRectTests
    {
        [Test]
        public void Drag_normalizes_to_min_size()
        {
            Assert.IsTrue(LotTileRect.TryFromDrag(3, 4, 3, 4, 3, 3, out int x, out int y, out int w, out int h));
            Assert.AreEqual(3, x);
            Assert.AreEqual(4, y);
            Assert.AreEqual(3, w);
            Assert.AreEqual(3, h);
        }

        [Test]
        public void Drag_expands_swapped_corners()
        {
            LotTileRect.TryFromDrag(5, 1, 2, 4, 2, 2, out int x, out int y, out int w, out int h);
            Assert.AreEqual(2, x);
            Assert.AreEqual(1, y);
            Assert.AreEqual(4, w);
            Assert.AreEqual(4, h);
        }

        [Test]
        public void FitsLot_rejects_out_of_bounds()
        {
            Assert.IsFalse(LotTileRect.FitsLot(14, 10, 12, 0, 5, 5));
            Assert.IsTrue(LotTileRect.FitsLot(14, 10, 0, 0, 3, 3));
        }
    }
}