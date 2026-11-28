using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SellGarageClutter(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            LotClutterState piece = null;
            for (int i = 0; i < lab.Clutter.Count; i++)
                if (lab.Clutter[i].Id == cmd.TargetId) { piece = lab.Clutter[i]; break; }
            if (piece == null) return Fail(CommandError.UnknownId, "There is no clutter item " + cmd.TargetId + ".");
            if (piece.Sold > 0) return Fail(CommandError.InvalidArgument, "That clutter is already gone.");
            GarageClutterConfig cfg = null;
            for (int i = 0; i < c.GarageDraw.Clutter.Count; i++)
                if (c.GarageDraw.Clutter[i].Id == piece.ClutterId) { cfg = c.GarageDraw.Clutter[i]; break; }
            if (cfg == null) return Fail(CommandError.UnknownId, "Unknown clutter type.");
            piece.Sold = 1;
            if (cfg.SellCash > 0) Finance.Earn(lab, LedgerLine.Construction, cfg.SellCash);
            Events.Add(state, EventKind.RoomDemolished, lab.Id, piece.Id, cfg.SellCash, piece.ClutterId);
            return CommandResult.Done(-1);
        }

        static CommandResult PlaceRoomDoor(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            RoomState room = lab.Room(cmd.TargetId);
            if (room == null) return Fail(CommandError.UnknownId, "There is no room " + cmd.TargetId + ".");
            if (room.DoorPlaced) return Fail(CommandError.InvalidArgument, "That room already has a door.");
            if (!GarageDrawRoom.DoorOnEdge(room, cmd.X, cmd.Y))
                return Fail(CommandError.InvalidArgument, "The door must sit on an outer edge tile of the room.");
            long cost = c.GarageDraw.DoorCost;
            if (lab.Cash < cost) return NeedCash(lab, cost);
            room.DoorLocalX = cmd.X;
            room.DoorLocalY = cmd.Y;
            Finance.Spend(lab, LedgerLine.Construction, cost);
            return CommandResult.Done(-1);
        }

        static CommandResult PlaceDesk(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            RoomState room = lab.Room(cmd.TargetId);
            if (room == null) return Fail(CommandError.UnknownId, "There is no room " + cmd.TargetId + ".");
            if (!GarageDrawRoom.RoomReadyForUse(c, lab, room))
                return Fail(CommandError.RoomNeedsDoor, "That room still needs a door before you can place desks.");
            RoomTypeConfig type = c.RoomType(room.TypeId);
            if (type == null || string.IsNullOrEmpty(type.DeskFamily))
                return Fail(CommandError.InvalidArgument, "You cannot place desks in that room.");
            if (cmd.X < 0 || cmd.Y < 0 || cmd.X >= room.Width || cmd.Y >= room.Height)
                return Fail(CommandError.OutOfBounds, "That tile is not inside the room.");
            if (GarageDrawRoom.DeskAt(lab, room.Id, cmd.X, cmd.Y) != null)
                return Fail(CommandError.InvalidArgument, "There is already a desk there.");
            DeskLineConfig line = GarageDrawRoom.DeskLine(c, cmd.Id);
            if (line == null) return Fail(CommandError.UnknownId, "There is no desk line \"" + cmd.Id + "\".");
            if (line.Family != type.DeskFamily)
                return Fail(CommandError.InvalidArgument, "That desk does not belong in this room.");
            if (lab.Cash < line.PlaceCost) return NeedCash(lab, line.PlaceCost);
            var desk = new RoomDeskState();
            desk.Id = state.TakeId();
            desk.RoomId = room.Id;
            desk.LocalX = cmd.X;
            desk.LocalY = cmd.Y;
            desk.DeskLineId = line.Id;
            lab.Desks.Add(desk);
            Finance.Spend(lab, LedgerLine.Construction, line.PlaceCost);
            return CommandResult.Done(desk.Id);
        }

        static CommandResult RemoveDesk(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            RoomDeskState desk = null;
            for (int i = 0; i < lab.Desks.Count; i++)
                if (lab.Desks[i].Id == cmd.TargetId) { desk = lab.Desks[i]; break; }
            if (desk == null) return Fail(CommandError.UnknownId, "There is no desk " + cmd.TargetId + ".");
            for (int i = 0; i < lab.Staff.Count; i++)
            {
                StaffMember p = lab.Staff[i];
                if (p.SeatRoomId == desk.RoomId && p.SeatDeskLocalX == desk.LocalX && p.SeatDeskLocalY == desk.LocalY)
                    return Fail(CommandError.StaffNeedSeats, "Someone is still sitting at that desk.");
            }
            lab.Desks.Remove(desk);
            return CommandResult.Done(-1);
        }

        static CommandResult SeatStaffAtDesk(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            StaffMember person = null;
            for (int i = 0; i < lab.Staff.Count; i++)
                if (lab.Staff[i].Id == cmd.TargetId) { person = lab.Staff[i]; break; }
            if (person == null) return Fail(CommandError.UnknownId, "There is no staff member " + cmd.TargetId + ".");
            RoomState room = lab.Room(cmd.X);
            if (room == null) return Fail(CommandError.UnknownId, "There is no room " + cmd.X + ".");
            if (!GarageDrawRoom.RoomReadyForUse(c, lab, room))
                return Fail(CommandError.RoomNeedsDoor, "That room still needs a door.");
            RoomTypeConfig type = c.RoomType(room.TypeId);
            if (type == null || type.SeatsRole != person.RoleId)
                return Fail(CommandError.InvalidArgument, "That person cannot work in that room.");
            RoomDeskState desk = GarageDrawRoom.DeskAt(lab, room.Id, cmd.Y, cmd.Width);
            if (desk == null) return Fail(CommandError.InvalidArgument, "There is no desk at that spot.");
            DeskLineConfig line = GarageDrawRoom.DeskLine(c, desk.DeskLineId);
            if (line == null || line.SeatsRole != person.RoleId)
                return Fail(CommandError.InvalidArgument, "That desk does not match their job.");
            for (int i = 0; i < lab.Staff.Count; i++)
            {
                StaffMember other = lab.Staff[i];
                if (other.Id == person.Id) continue;
                if (other.SeatRoomId == room.Id && other.SeatDeskLocalX == desk.LocalX && other.SeatDeskLocalY == desk.LocalY)
                    return Fail(CommandError.InvalidArgument, "Someone else is already at that desk.");
            }
            person.SeatRoomId = room.Id;
            person.SeatDeskLocalX = desk.LocalX;
            person.SeatDeskLocalY = desk.LocalY;
            return CommandResult.Done(-1);
        }

        static CommandResult PaintRoomTiles(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            RoomTypeConfig type = c.RoomType(cmd.Id);
            if (type == null) return Fail(CommandError.UnknownId, "There is no room type \"" + cmd.Id + "\".");
            if (!GarageDrawRoom.BuildMenuOpen(c, lab, type))
                return Fail(CommandError.DrawMenuLocked, type.Name + " is not on the build menu yet.");
            if (!EraSystem.RoomOpen(c, type, state.Tick))
                return Fail(CommandError.EraLocked, type.Name + " is not around yet.");
            List<int> flat = GarageDrawRoom.ParseTileList(cmd.Ids);
            if (flat.Count < 4 || flat.Count % 2 != 0)
                return Fail(CommandError.InvalidArgument, "Paint at least two connected tiles.");
            int minX = flat[0], maxX = flat[0], minY = flat[1], maxY = flat[1];
            for (int i = 0; i < flat.Count; i += 2)
            {
                int tx = flat[i], ty = flat[i + 1];
                if (tx < minX) minX = tx;
                if (tx > maxX) maxX = tx;
                if (ty < minY) minY = ty;
                if (ty > maxY) maxY = ty;
            }
            int w = maxX - minX + 1, h = maxY - minY + 1;
            if (!GarageDrawRoom.MeetsDrawMinimum(type, w, h))
                return Fail(CommandError.TooSmall, "That shape is too small for a " + type.Name + ".");
            return BuildRoomRect(c, state, lab, type, minX, minY, w, h, flat);
        }

        internal static CommandResult BuildRoomRect(GameConfig c, GameState state, LabState lab, RoomTypeConfig type, int x, int y, int w, int h, List<int> paintTiles)
        {
            LotConfig lot = c.Lot(lab.LotId);
            if (!LotGrid.InBounds(lot, x, y, w, h))
                return Fail(CommandError.OutOfBounds, "That does not fit on the lot.");
            if (!LotGrid.IsFree(lab.Rooms, x, y, w, h))
                return Fail(CommandError.Overlap, "Another room is in the way.");
            if (GarageDrawRoom.RectBlockedByClutter(lab, x, y, w, h))
                return Fail(CommandError.Overlap, "Clutter is in the way. Sell it first.");
            int tiles = w * h;
            long cost = type.BuildCostPerTile * tiles;
            if (lab.Cash < cost) return NeedCash(lab, cost);
            var room = new RoomState();
            room.Id = state.TakeId();
            room.TypeId = type.Id;
            room.X = x;
            room.Y = y;
            room.Width = w;
            room.Height = h;
            if (GarageDrawRoom.Active(c, lab)) { room.DoorLocalX = -1; room.DoorLocalY = -1; }
            lab.Rooms.Add(room);
            Finance.Spend(lab, LedgerLine.Construction, cost);
            Events.Add(state, EventKind.RoomBuilt, lab.Id, room.Id, cost, type.Id);
            return CommandResult.Done(room.Id);
        }
    }
}