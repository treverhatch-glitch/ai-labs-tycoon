using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>A paid attack on one model before or after launch. See <see cref="RedTeamConfig"/>. Only the player's lab does it.</summary>
    public static class RedTeam
    {
        public static bool Enabled(GameConfig c) { return c.RedTeam.Weeks > 0; }

        public static bool Active(GameState state, ModelState model) { return model.RedTeamUntil > state.Tick; }

        public static bool ActiveAt(int tick, ModelState model) { return model.RedTeamUntil > tick; }

        public static int Attackers(GameConfig c, LabState lab)
        {
            if (c.RedTeam.RoleId == "") return c.RedTeam.MinStaff;
            int n = 0;
            for (int i = 0; i < lab.Staff.Count; i++) if (lab.Staff[i].RoleId == c.RedTeam.RoleId) n++;
            return n;
        }

        public static long Cost(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            return MathI.Max(c.RedTeam.MinCost, MarketSystem.ModelRevenue(state, lab.Id, model.Id) * c.RedTeam.RevenueWeeks);
        }

        /// <summary>The chance (a negative number, in permille) a red-teamed model's weekly incident chance loses; zero when the model is not covered.</summary>
        public static int IncidentPermille(GameConfig c, int tick, ModelState model, int chanceBefore)
        {
            if (!ActiveAt(tick, model)) return 0;
            return -(int)MathI.Min(chanceBefore, MathI.MulPermille(chanceBefore, c.RedTeam.IncidentCutPermille));
        }

        /// <summary>How much of the gap between the shown and real alignment an attack would close right now.</summary>
        public static int GapClosed(GameConfig c, ModelState model)
        {
            return (int)MathI.MulPermille(SafetySystem.AlignmentGap(c, model), c.RedTeam.GapClosePermille);
        }

        public static void Run(GameConfig c, GameState state, LabState lab, ModelState model, long cost)
        {
            int closed = GapClosed(c, model);
            model.DisplayedAlignment = (int)MathI.Max(model.Alignment, model.DisplayedAlignment - closed);
            model.Alignment = (int)MathI.Min(MathI.Permille, (long)model.Alignment + c.RedTeam.AlignmentGain);
            model.DisplayedAlignment = (int)MathI.Min(MathI.Permille, (long)model.DisplayedAlignment + c.RedTeam.AlignmentGain);
            model.RedTeamUntil = state.Tick + c.RedTeam.Weeks;
            Finance.Spend(lab, LedgerLine.Training, cost);
            SafetyCulture.Move(c, lab, c.RedTeam.CulturePoints);
            Events.Add(state, EventKind.RedTeamRan, lab.Id, model.Id, cost, model.Name);
        }
    }
}
