using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Empty-garage start: clutter, draw-room build menu, doors, desks and demolish refund (GD-6 step 1).</summary>
    public sealed class GarageDrawConfig : ISync
    {
        /// <summary>When true, a new player game starts with draw-room rules (no pre-spawned rooms).</summary>
        public int Enabled;
        public long DoorCost = 200;
        /// <summary>Share of tile build cost returned when a room is demolished (500 = 50%).</summary>
        public int DemolishRefundPermille = 500;
        /// <summary>Room type ids on the day-one build menu when draw-room is active.</summary>
        public List<string> DayOneRoomTypeIds = new List<string>();
        public List<GarageClutterConfig> Clutter = new List<GarageClutterConfig>();
        public List<DeskLineConfig> DeskLines = new List<DeskLineConfig>();

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Long("doorCost", ref DoorCost);
            s.Int("demolishRefundPermille", ref DemolishRefundPermille);
            s.StrList("dayOneRoomTypeIds", ref DayOneRoomTypeIds);
            s.ObjList("clutter", ref Clutter);
            s.ObjList("deskLines", ref DeskLines);
        }
    }

  public sealed class GarageClutterConfig : ISync, IHasId
    {
        public string IdValue = "";
        public int X;
        public int Y;
        public int Width = 1;
        public int Height = 1;
        public long SellCash;
        /// <summary>When refusing to sell pays a morale permille bonus to the founder for <see cref="RefuseMoraleWeeks"/> weeks.</summary>
        public int RefuseMoralePermille;
        public int RefuseMoraleWeeks;
        public string Joke = "";

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Int("x", ref X);
            s.Int("y", ref Y);
            s.Int("width", ref Width);
            s.Int("height", ref Height);
            s.Long("sellCash", ref SellCash);
            s.Int("refuseMoralePermille", ref RefuseMoralePermille);
            s.Int("refuseMoraleWeeks", ref RefuseMoraleWeeks);
            s.Str("joke", ref Joke);
        }
    }

    public sealed class DeskLineConfig : ISync, IHasId
    {
        public string IdValue = "";
        /// <summary>Matches <see cref="RoomTypeConfig.DeskFamily"/> on the room type.</summary>
        public string Family = "";
        public string SeatsRole = "";
        public int Tier = 1;
        public long PlaceCost;
        public string Name = "";

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("family", ref Family);
            s.Str("seatsRole", ref SeatsRole);
            s.Int("tier", ref Tier);
            s.Long("placeCost", ref PlaceCost);
            s.Str("name", ref Name);
        }
    }
}