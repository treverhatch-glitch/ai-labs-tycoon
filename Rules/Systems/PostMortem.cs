using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Public post-mortems. See <see cref="PostMortemConfig"/>. Only the player's lab publishes them.</summary>
    public static class PostMortem
    {
        public const int Summary = 1;
        public const int Full = 2;

        public static bool Enabled(GameConfig c) { return c.PostMortem.WindowWeeks > 0; }

        public static bool Published(ModelState model) { return model.PostMortemDone > 0; }

        /// <summary>True when the model has had an incident that is still within the window and has no write-up yet.</summary>
        public static bool Open(GameConfig c, GameState state, ModelState model)
        {
            return Enabled(c) && model.Incidents > 0 && !Published(model) && state.Tick - model.LastIncidentTick <= c.PostMortem.WindowWeeks;
        }

        public static int SafetyStaff(GameConfig c, LabState lab) { return LabQueries.StaffByFunction(c, lab, RoleFunction.Safety); }

        public static long Cost(GameConfig c, int depth) { return depth == Full ? c.PostMortem.FullCost : c.PostMortem.SummaryCost; }

        public static int RecoverPermille(GameConfig c, int depth)
        {
            return (int)MathI.Clamp(depth == Full ? c.PostMortem.FullRecoverPermille : c.PostMortem.SummaryRecoverPermille, 0, MathI.Permille);
        }

        /// <summary>The reputation a write-up of that depth would win back right now.</summary>
        public static int Recovery(GameConfig c, ModelState model, int depth)
        {
            return (int)MathI.MulPermille(MathI.Max(0, model.IncidentRepLoss), RecoverPermille(c, depth));
        }

        public static void Publish(GameConfig c, GameState state, LabState lab, ModelState model, int depth, long cost)
        {
            if (cost > 0) Finance.Spend(lab, LedgerLine.Fines, cost);
            int back = Recovery(c, model, depth);
            lab.Reputation = (int)MathI.Clamp(lab.Reputation + back, 0, MathI.Permille);
            if (depth == Full) model.DisplayedAlignment = model.Alignment;
            model.PostMortemDone = depth;
            GameEvent e = Events.Add(state, EventKind.PostMortemPublished, lab.Id, model.Id, back, model.Name);
            e.Pick = depth == Full ? 1 : 0;
        }
    }
}
