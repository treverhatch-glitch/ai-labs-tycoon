using AiLabs.Rules.Config;

namespace AiLabs.Rules.Systems
{
    /// <summary>The calendar: a tick is one week, and the config says how many weeks a month and a year have.</summary>
    public static class GameDate
    {
        public static int Year(CalendarConfig c, int tick) { return c.StartYear + tick / c.WeeksPerYear; }

        /// <summary>1 to monthsPerYear.</summary>
        public static int Month(CalendarConfig c, int tick) { return tick % c.WeeksPerYear / c.WeeksPerMonth + 1; }

        /// <summary>1 to weeksPerMonth.</summary>
        public static int Week(CalendarConfig c, int tick) { return tick % c.WeeksPerMonth + 1; }

        public static bool IsMonthStart(CalendarConfig c, int tick) { return tick % c.WeeksPerMonth == 0; }

        public static bool IsMonthEnd(CalendarConfig c, int tick) { return (tick + 1) % c.WeeksPerMonth == 0; }

        public static bool IsYearEnd(CalendarConfig c, int tick) { return (tick + 1) % c.WeeksPerYear == 0; }

        /// <summary>The first tick of a year.</summary>
        public static int TickOfYear(CalendarConfig c, int year) { return (year - c.StartYear) * c.WeeksPerYear; }

        public static string Label(CalendarConfig c, int tick)
        {
            return Year(c, tick) + " M" + Month(c, tick) + " W" + Week(c, tick);
        }
    }
}
