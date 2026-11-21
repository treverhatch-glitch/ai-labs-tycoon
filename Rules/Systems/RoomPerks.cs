using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// What the later rooms do beyond seating people (a research floor speeds research, a sandbox wing catches incidents). A perk is the
    /// tiles of its room times a share, held under a cap, worked out from the rooms each time; a room that is sold stops helping at once
    /// and nothing is saved.
    /// </summary>
    public static class RoomPerks
    {
        /// <summary>One perk as a screen would list it: the room, how many tiles the lab has, what that gives, and whether it has hit the cap.</summary>
        public struct RoomPerkLine
        {
            public RoomPerkConfig Perk;
            public int Tiles;
            public int Permille;
            public bool AtCap;
        }

        public static int Tiles(LabState lab, string roomTypeId)
        {
            int n = 0;
            for (int i = 0; i < lab.Rooms.Count; i++) if (lab.Rooms[i].TypeId == roomTypeId) n += lab.Rooms[i].Tiles;
            return n;
        }

        public static int Permille(GameConfig c, LabState lab, RoomPerkConfig perk)
        {
            long raw = (long)Tiles(lab, perk.RoomId) * perk.PerTilePermille;
            return (int)MathI.Min(raw, perk.CapPermille);
        }

        public static int Permille(GameConfig c, LabState lab, RoomPerkKind kind)
        {
            long total = 0;
            for (int i = 0; i < c.RoomPerks.Count; i++)
                if (c.RoomPerks[i].Kind == kind) total += Permille(c, lab, c.RoomPerks[i]);
            return (int)total;
        }

        public static int SectorAppealPermille(GameConfig c, LabState lab, string sectorId)
        {
            long total = 0;
            for (int i = 0; i < c.RoomPerks.Count; i++)
            {
                RoomPerkConfig perk = c.RoomPerks[i];
                if (perk.Kind != RoomPerkKind.SectorAppeal) continue;
                if (perk.Sectors.Count > 0 && !perk.Sectors.Contains(sectorId)) continue;
                total += Permille(c, lab, perk);
            }
            return (int)total;
        }

        /// <summary>Every perk of the rooms the lab actually has, for a "what do my rooms do" screen.</summary>
        public static List<RoomPerkLine> Lines(GameConfig c, LabState lab)
        {
            var lines = new List<RoomPerkLine>();
            for (int i = 0; i < c.RoomPerks.Count; i++)
            {
                RoomPerkConfig perk = c.RoomPerks[i];
                int tiles = Tiles(lab, perk.RoomId);
                if (tiles < 1) continue;
                var line = new RoomPerkLine();
                line.Perk = perk;
                line.Tiles = tiles;
                line.Permille = Permille(c, lab, perk);
                line.AtCap = line.Permille >= perk.CapPermille;
                lines.Add(line);
            }
            return lines;
        }
    }
}
