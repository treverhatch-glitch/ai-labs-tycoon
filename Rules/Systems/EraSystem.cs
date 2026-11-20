using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// Eras and the dated world. Everything here is a read of the calendar plus the era table, so the
    /// state holds no "current era": the year says which era it is.
    /// </summary>
    public static class EraSystem
    {
        public static int IndexAtYear(GameConfig c, int year)
        {
            int at = 0;
            for (int i = 0; i < c.Eras.Count; i++) if (c.Eras[i].StartYear <= year) at = i;
            return at;
        }

        public static EraConfig AtYear(GameConfig c, int year) { return c.Eras[IndexAtYear(c, year)]; }

        public static EraConfig AtTick(GameConfig c, int tick) { return AtYear(c, GameDate.Year(c.Calendar, tick)); }

        public static int IndexOf(GameConfig c, string eraId)
        {
            for (int i = 0; i < c.Eras.Count; i++) if (c.Eras[i].Id == eraId) return i;
            return -1;
        }

        /// <summary>The first tick of an era.</summary>
        public static int StartTick(GameConfig c, EraConfig era) { return GameDate.TickOfYear(c.Calendar, era.StartYear); }

        /// <summary>True once the era a tech, room or sector names has begun.</summary>
        public static bool Open(GameConfig c, string eraId, int tick)
        {
            EraConfig era = c.Era(eraId);
            return era != null && GameDate.Year(c.Calendar, tick) >= era.StartYear;
        }

        public static bool TechOpen(GameConfig c, TechConfig tech, int tick) { return Open(c, tech.Era, tick); }

        public static bool RoomOpen(GameConfig c, RoomTypeConfig room, int tick) { return Open(c, room.Era, tick); }

        /// <summary>A model line's compute need for a run that starts at this tick: the line's listed cost scaled by the era.</summary>
        public static long RunCompute(GameConfig c, ModelLineConfig line, int tick)
        {
            return MathI.MulPermille(line.ComputeRequired, AtTick(c, tick).TrainingCostPermille);
        }

        /// <summary>The cash a run that starts at this tick costs to prepare.</summary>
        public static long RunPrepCost(GameConfig c, ModelLineConfig line, int tick)
        {
            return MathI.MulPermille(line.PrepCost, AtTick(c, tick).TrainingCostPermille);
        }

        /// <summary>The growth rate, in permille, a sector's pool gets at the end of this year: its listed rate, slowed by the decay for every year since it opened.</summary>
        public static int SectorGrowthPermille(SectorConfig sector, int year)
        {
            long growth = sector.GrowthPermillePerYear;
            for (int y = sector.OpensYear; y < year && growth > 0; y++) growth = growth * sector.GrowthDecayPermille / MathI.Permille;
            return (int)growth;
        }

        /// <summary>The first tick of the month a world event lands in.</summary>
        public static int EventTick(GameConfig c, WorldEventConfig e)
        {
            return GameDate.TickOfYear(c.Calendar, e.Year) + (e.Month - 1) * c.Calendar.WeeksPerMonth;
        }

        /// <summary>
        /// Announces the era that begins this week, then every world event dated this week. An event moves
        /// its sector's market once; nothing is rolled, so adding events never shifts any dice.
        /// </summary>
        public static void RunWeek(GameConfig c, GameState state)
        {
            int tick = state.Tick;
            for (int i = 0; i < c.Eras.Count; i++)
                if (StartTick(c, c.Eras[i]) == tick) Events.Add(state, EventKind.EraBegan, -1, -1, c.Eras[i].StartYear, c.Eras[i].Id);

            for (int i = 0; i < c.WorldEvents.Count; i++)
            {
                WorldEventConfig e = c.WorldEvents[i];
                if (EventTick(c, e) != tick) continue;
                if (!string.IsNullOrEmpty(e.SectorId))
                {
                    SectorState sector = state.Sector(e.SectorId);
                    if (sector != null) sector.PoolPerWeek = MathI.MulPermille(sector.PoolPerWeek, e.PoolPermille);
                }
                Events.Add(state, EventKind.WorldNews, -1, -1, e.PoolPermille, e.Id);
            }
        }
    }
}
