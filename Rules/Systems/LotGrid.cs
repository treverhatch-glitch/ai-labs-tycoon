using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Where rooms can stand on a lot. Rooms are rectangles of tiles and never overlap.</summary>
    public static class LotGrid
    {
        public static bool InBounds(LotConfig lot, int x, int y, int w, int h)
        {
            return x >= 0 && y >= 0 && w > 0 && h > 0 && x + w <= lot.Width && y + h <= lot.Height;
        }

        public static bool Overlaps(RoomState room, int x, int y, int w, int h)
        {
            return x < room.X + room.Width && room.X < x + w && y < room.Y + room.Height && room.Y < y + h;
        }

        public static bool IsFree(List<RoomState> rooms, int x, int y, int w, int h)
        {
            for (int i = 0; i < rooms.Count; i++) if (Overlaps(rooms[i], x, y, w, h)) return false;
            return true;
        }

        public static bool CanPlace(LotConfig lot, List<RoomState> rooms, int x, int y, int w, int h)
        {
            return InBounds(lot, x, y, w, h) && IsFree(rooms, x, y, w, h);
        }

        /// <summary>The first free spot for a room of this size, scanning rows top to bottom. False when none is left.</summary>
        public static bool FindFreeRect(LotConfig lot, List<RoomState> rooms, int w, int h, out int x, out int y)
        {
            for (y = 0; y + h <= lot.Height; y++)
                for (x = 0; x + w <= lot.Width; x++)
                    if (IsFree(rooms, x, y, w, h)) return true;
            x = -1;
            y = -1;
            return false;
        }

        public static int FreeTiles(LotConfig lot, List<RoomState> rooms)
        {
            int used = 0;
            for (int i = 0; i < rooms.Count; i++) used += rooms[i].Tiles;
            return lot.Width * lot.Height - used;
        }
    }
}
