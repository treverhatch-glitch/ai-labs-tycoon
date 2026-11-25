using System;

namespace AiLabs.Unity.Hq
{
    /// <summary>Snaps a drag on the lot grid to a build rectangle (presentation helper, not rules).</summary>
    public static class LotTileRect
    {
        public static bool TryFromDrag(int ax, int ay, int bx, int by, int minWidth, int minHeight, out int x, out int y, out int width, out int height)
        {
            x = Math.Min(ax, bx);
            y = Math.Min(ay, by);
            int x2 = Math.Max(ax, bx);
            int y2 = Math.Max(ay, by);
            width = x2 - x + 1;
            height = y2 - y + 1;
            if (width < minWidth) width = minWidth;
            if (height < minHeight) height = minHeight;
            return width >= minWidth && height >= minHeight;
        }

        public static bool FitsLot(int lotWidth, int lotHeight, int x, int y, int width, int height)
        {
            return x >= 0 && y >= 0 && width > 0 && height > 0 && x + width <= lotWidth && y + height <= lotHeight;
        }
    }
}