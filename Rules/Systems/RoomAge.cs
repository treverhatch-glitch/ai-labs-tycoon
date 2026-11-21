using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Rooms that age. See <see cref="AgeingConfig"/>. Only the player's rooms age.</summary>
    public static class RoomAge
    {
        public struct Row
        {
            public int RoomId;
            public string TypeId;
            public int Eras;
            public string StageId;
            public int StageIndex;
            public int UpkeepExtraPermille;
            public int MoralePenalty;
            public long RefurbishCost;
        }

        public struct Report
        {
            public List<Row> Rooms;
            public int MoralePenalty;
            public long ExtraUpkeepPerWeek;
        }

        static bool Ages(LabState lab) { return lab.Controller == LabController.Player; }

        public static int StageIndex(GameConfig c, int eras)
        {
            int best = 0;
            for (int i = 0; i < c.Ageing.Stages.Count; i++)
                if (c.Ageing.Stages[i].MinEras <= eras && c.Ageing.Stages[i].MinEras >= c.Ageing.Stages[best].MinEras) best = i;
            return best;
        }

        public static AgeStageConfig Stage(GameConfig c, int eras) { return c.Ageing.Stages[StageIndex(c, eras)]; }

        public static int Eras(LabState lab, RoomState room) { return Ages(lab) ? room.EraAge : 0; }

        public static int UpkeepExtraPermille(GameConfig c, LabState lab, RoomState room)
        {
            return Stage(c, Eras(lab, room)).UpkeepExtraPermille;
        }

        public static long RefurbishCost(GameConfig c, RoomState room)
        {
            RoomTypeConfig type = c.RoomType(room.TypeId);
            if (type == null) return 0;
            return MathI.MulPermille(type.BuildCostPerTile * room.Tiles, c.Ageing.RefurbishCostPermille);
        }

        /// <summary>The morale the lab's dated rooms take off where its people settle: the tile-weighted average over the rooms that seat people.</summary>
        public static int MoralePenalty(GameConfig c, LabState lab)
        {
            if (!Ages(lab)) return 0;
            long weighted = 0;
            long tiles = 0;
            for (int i = 0; i < lab.Rooms.Count; i++)
            {
                RoomState room = lab.Rooms[i];
                RoomTypeConfig type = c.RoomType(room.TypeId);
                if (type == null || string.IsNullOrEmpty(type.SeatsRole)) continue;
                weighted += (long)Stage(c, room.EraAge).MoralePenalty * room.Tiles;
                tiles += room.Tiles;
            }
            if (tiles == 0) return 0;
            return (int)MathI.Min(c.Ageing.MaxMoralePenalty, weighted / tiles);
        }

        public static Report Of(GameConfig c, LabState lab)
        {
            var r = new Report();
            r.Rooms = new List<Row>();
            for (int i = 0; i < lab.Rooms.Count; i++)
            {
                RoomState room = lab.Rooms[i];
                RoomTypeConfig type = c.RoomType(room.TypeId);
                int eras = Eras(lab, room);
                int stage = StageIndex(c, eras);
                var row = new Row();
                row.RoomId = room.Id;
                row.TypeId = room.TypeId;
                row.Eras = eras;
                row.StageIndex = stage;
                row.StageId = c.Ageing.Stages[stage].IdValue;
                row.UpkeepExtraPermille = c.Ageing.Stages[stage].UpkeepExtraPermille;
                row.MoralePenalty = c.Ageing.Stages[stage].MoralePenalty;
                row.RefurbishCost = eras > 0 ? RefurbishCost(c, room) : 0;
                r.Rooms.Add(row);
                if (type != null) r.ExtraUpkeepPerWeek += MathI.MulPermille(type.UpkeepPerTileWeek * room.Tiles, row.UpkeepExtraPermille);
            }
            r.MoralePenalty = MoralePenalty(c, lab);
            return r;
        }

        /// <summary>A new era begins: every room of the player's labs is one era older, and a room that has just got worse starts to be talked about.</summary>
        public static void OnEraBegan(GameConfig c, GameState state)
        {
            for (int l = 0; l < state.Labs.Count; l++)
            {
                LabState lab = state.Labs[l];
                if (!Ages(lab)) continue;
                for (int i = 0; i < lab.Rooms.Count; i++)
                {
                    RoomState room = lab.Rooms[i];
                    int before = StageIndex(c, room.EraAge);
                    room.EraAge++;
                    int after = StageIndex(c, room.EraAge);
                    AgeStageConfig stage = c.Ageing.Stages[after];
                    if (after != before && (stage.UpkeepExtraPermille > 0 || stage.MoralePenalty > 0))
                        Events.Add(state, EventKind.RoomDated, lab.Id, room.Id, room.EraAge, room.TypeId);
                }
            }
        }
    }
}
