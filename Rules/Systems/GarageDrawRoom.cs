using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Empty garage start: build menu, doors, desks, clutter and seating (GD-6 step 1).</summary>
    public static class GarageDrawRoom
    {
        public static bool ConfigOn(GameConfig c) => c.GarageDraw != null && c.GarageDraw.Enabled > 0;

        public static bool Active(GameConfig c, LabState lab) => ConfigOn(c) && lab != null && lab.GarageDrawActive > 0;

        public static bool DayOneType(GameConfig c, string roomTypeId)
        {
            if (!ConfigOn(c)) return true;
            for (int i = 0; i < c.GarageDraw.DayOneRoomTypeIds.Count; i++)
                if (c.GarageDraw.DayOneRoomTypeIds[i] == roomTypeId) return true;
            return false;
        }

        public static bool BuildMenuOpen(GameConfig c, LabState lab, RoomTypeConfig type)
        {
            if (!Active(c, lab)) return true;
            return DayOneType(c, type.Id);
        }

        public static bool TileBlockedByClutter(LabState lab, int x, int y)
        {
            for (int i = 0; i < lab.Clutter.Count; i++)
            {
                LotClutterState piece = lab.Clutter[i];
                if (piece.Sold > 0) continue;
                if (x >= piece.X && y >= piece.Y && x < piece.X + piece.Width && y < piece.Y + piece.Height) return true;
            }
            return false;
        }

        public static bool RectBlockedByClutter(LabState lab, int x, int y, int w, int h)
        {
            for (int ty = y; ty < y + h; ty++)
                for (int tx = x; tx < x + w; tx++)
                    if (TileBlockedByClutter(lab, tx, ty)) return true;
            return false;
        }

        public static int ComfortMinTiles { get { return 4; } }

        public static int StaffedMinBounding(GameConfig c, RoomTypeConfig type)
        {
            if (type.Purpose == RoomPurpose.Comfort || type.Purpose == RoomPurpose.Compute) return 4;
            return 9;
        }

        public static bool MeetsDrawMinimum(RoomTypeConfig type, int w, int h)
        {
            int area = w * h;
            if (type.Purpose == RoomPurpose.Comfort || type.Purpose == RoomPurpose.Compute)
                return w >= type.MinWidth && h >= type.MinHeight && area >= ComfortMinTiles;
            return w >= type.MinWidth && h >= type.MinHeight && w * h >= StaffedMinBounding(c: null, type);
        }

        public static int EstimateSeats(GameConfig c, RoomTypeConfig type, int w, int h)
        {
            if (string.IsNullOrEmpty(type.DeskFamily) || type.TilesPerSeat < 1) return 0;
            return (w * h) / type.TilesPerSeat;
        }

        public static int OvercrowdSeats(GameConfig c, RoomTypeConfig type, int w, int h)
        {
            int comfortable = EstimateSeats(c, type, w, h);
            return comfortable > 0 ? comfortable + 1 : 0;
        }

        public static long TileBuildCost(RoomTypeConfig type, int tiles) { return type.BuildCostPerTile * tiles; }

        public static DeskLineConfig DeskLine(GameConfig c, string id)
        {
            for (int i = 0; i < c.GarageDraw.DeskLines.Count; i++)
                if (c.GarageDraw.DeskLines[i].Id == id) return c.GarageDraw.DeskLines[i];
            return null;
        }

        public static RoomDeskState DeskAt(LabState lab, int roomId, int localX, int localY)
        {
            for (int i = 0; i < lab.Desks.Count; i++)
            {
                RoomDeskState d = lab.Desks[i];
                if (d.RoomId == roomId && d.LocalX == localX && d.LocalY == localY) return d;
            }
            return null;
        }

        public static bool DoorOnEdge(RoomState room, int localX, int localY)
        {
            if (localX < 0 || localY < 0 || localX >= room.Width || localY >= room.Height) return false;
            return localX == 0 || localY == 0 || localX == room.Width - 1 || localY == room.Height - 1;
        }

        public static bool RoomReadyForUse(GameConfig c, LabState lab, RoomState room)
        {
            if (!Active(c, lab)) return true;
            return room.DoorPlaced;
        }

        public static int SeatsInRoomDraw(GameConfig c, LabState lab, RoomState room)
        {
            if (!RoomReadyForUse(c, lab, room)) return 0;
            RoomTypeConfig type = c.RoomType(room.TypeId);
            if (type == null || string.IsNullOrEmpty(type.DeskFamily)) return 0;
            int n = 0;
            for (int i = 0; i < lab.Desks.Count; i++)
            {
                RoomDeskState d = lab.Desks[i];
                if (d.RoomId != room.Id) continue;
                DeskLineConfig line = DeskLine(c, d.DeskLineId);
                if (line != null && line.Family == type.DeskFamily) n++;
            }
            return n;
        }

        public static bool StaffSeatedForWork(GameConfig c, LabState lab, StaffMember person)
        {
            if (!Active(c, lab)) return true;
            if (person.SeatRoomId < 0) return false;
            RoomState room = lab.Room(person.SeatRoomId);
            if (room == null || !RoomReadyForUse(c, lab, room)) return false;
            RoomTypeConfig type = c.RoomType(room.TypeId);
            if (type == null || type.SeatsRole != person.RoleId) return false;
            return DeskAt(lab, room.Id, person.SeatDeskLocalX, person.SeatDeskLocalY) != null;
        }

        public static void SpawnClutter(GameConfig c, GameState state, LabState lab)
        {
            if (!Active(c, lab)) return;
            for (int i = 0; i < c.GarageDraw.Clutter.Count; i++)
            {
                GarageClutterConfig cfg = c.GarageDraw.Clutter[i];
                var piece = new LotClutterState();
                piece.Id = state.TakeId();
                piece.ClutterId = cfg.Id;
                piece.X = cfg.X;
                piece.Y = cfg.Y;
                piece.Width = cfg.Width;
                piece.Height = cfg.Height;
                lab.Clutter.Add(piece);
            }
        }

        public static void SpawnFounderStaff(GameConfig c, GameState state, LabState lab, FounderConfig founder)
        {
            if (!Active(c, lab)) return;
            var person = new StaffMember();
            person.Id = state.TakeId();
            person.RoleId = founder == null ? "engineer" : founder.StartRoleId;
            person.TraitId = "steady";
            person.Morale = c.StaffPeople.MoraleBaseline;
            person.IsFounder = 1;
            lab.Staff.Add(person);
        }

        public static void SpawnFounderStaffNoPicker(GameConfig c, GameState state, LabState lab)
        {
            if (!Active(c, lab)) return;
            if (lab.Staff.Count > 0) return;
            var person = new StaffMember();
            person.Id = state.TakeId();
            person.RoleId = "engineer";
            person.TraitId = "steady";
            person.Morale = c.StaffPeople.MoraleBaseline;
            person.IsFounder = 1;
            lab.Staff.Add(person);
        }

        public static long RoomBuildCostPaid(RoomState room, RoomTypeConfig type) { return type.BuildCostPerTile * room.Tiles; }

        public static void RemoveDesksInRoom(LabState lab, int roomId)
        {
            for (int i = lab.Desks.Count - 1; i >= 0; i--)
                if (lab.Desks[i].RoomId == roomId) lab.Desks.RemoveAt(i);
            for (int i = 0; i < lab.Staff.Count; i++)
            {
                if (lab.Staff[i].SeatRoomId == roomId)
                {
                    lab.Staff[i].SeatRoomId = -1;
                    lab.Staff[i].SeatDeskLocalX = -1;
                    lab.Staff[i].SeatDeskLocalY = -1;
                }
            }
        }

        public static List<int> ParseTileList(List<string> ids)
        {
            var tiles = new List<int>();
            if (ids == null) return tiles;
            for (int i = 0; i < ids.Count; i++)
            {
                string part = ids[i];
                int comma = part.IndexOf(',');
                if (comma < 1) continue;
                int x, y;
                if (!int.TryParse(part.Substring(0, comma), out x)) continue;
                if (!int.TryParse(part.Substring(comma + 1), out y)) continue;
                tiles.Add(x);
                tiles.Add(y);
            }
            return tiles;
        }
    }
}