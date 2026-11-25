using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>A launch with some of the testing skipped. See <see cref="RushConfig"/>.</summary>
    public static class RushedLaunch
    {
        public static bool WasRushed(ModelState model) { return model.RushWeeks > 0; }

        static bool In(int tick, ModelState model, int weeks)
        {
            return model.RushWeeks > 0 && model.ReleasedTick >= 0 && tick < model.ReleasedTick + weeks;
        }

        /// <summary>Appeal as a share of its usual value, in permille: 1000 unless the launch window of a rushed release is open.</summary>
        public static int LaunchAppealPermille(GameConfig c, int tick, ModelState model)
        {
            if (!In(tick, model, c.Rush.LaunchWeeks)) return MathI.Permille;
            return (int)MathI.Max(1, MathI.Permille + (long)model.RushWeeks * c.Rush.LaunchAppealPermillePerWeek);
        }

        /// <summary>The permille added to the weekly incident chance.</summary>
        public static int IncidentPermille(GameConfig c, int tick, ModelState model)
        {
            if (!In(tick, model, c.Rush.IncidentWeeks)) return 0;
            return (int)MathI.Min(MathI.Permille, (long)model.RushWeeks * c.Rush.IncidentPermillePerWeek);
        }

        /// <summary>Weeks left in the launch window, or 0.</summary>
        public static int LaunchWeeksLeft(GameConfig c, int tick, ModelState model)
        {
            if (!In(tick, model, c.Rush.LaunchWeeks)) return 0;
            return (int)(model.ReleasedTick + c.Rush.LaunchWeeks - tick);
        }
    }
}
