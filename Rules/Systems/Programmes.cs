using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>Multi-stage research programmes. See <see cref="ProgrammesConfig"/>. Only the player's lab.</summary>
    public static class Programmes
    {
        public const int Running = 1;
        public const int Failed = 2;
        public const int Choosing = 3;

        public static bool Enabled(GameConfig c) { return c.Programmes.Enabled > 0; }

        public static bool Active(LabState lab) { return lab.PgTech > 0; }

        public static TechConfig Tech(GameConfig c, LabState lab) { return c.Research.Techs[lab.PgTech - 1]; }

        public static long StageCost(GameConfig c, LabState lab, bool bold)
        {
            return MathI.MulPermille(Tech(c, lab).Cost, bold ? c.Programmes.BoldCostPermille : c.Programmes.SafeCostPermille);
        }

        public static void Start(GameConfig c, GameState state, LabState lab, int index)
        {
            lab.PgTech = index + 1;
            lab.PgDone = 0;
            lab.PgPhase = Choosing;
            lab.PgLeft = 0;
            lab.PgBold = 0;
            lab.PgInsight = 0;
            lab.PgSpent = 0;
            Events.Add(state, EventKind.ProgrammeStarted, lab.Id, index, 0, c.Research.Techs[index].Id);
        }

        public static void Run(GameConfig c, LabState lab, bool bold)
        {
            long cost = StageCost(c, lab, bold);
            lab.ResearchPoints -= cost;
            lab.PgSpent += cost;
            lab.PgBold = bold ? 1 : 0;
            lab.PgLeft = bold ? c.Programmes.BoldWeeks : c.Programmes.SafeWeeks;
            if (Pods.Sharp(c, lab)) lab.PgLeft = (int)MathI.Max(1, lab.PgLeft - c.Pods.ProgrammeWeeksSaved);
            lab.PgPhase = Running;
        }

        static void Reset(LabState lab)
        {
            lab.PgTech = 0;
            lab.PgDone = 0;
            lab.PgPhase = 0;
            lab.PgLeft = 0;
            lab.PgBold = 0;
            lab.PgInsight = 0;
            lab.PgSpent = 0;
        }

        public static void Abandon(GameConfig c, GameState state, LabState lab)
        {
            long refund = MathI.MulPermille(lab.PgSpent, c.Programmes.RefundPermille);
            lab.ResearchPoints += refund;
            lab.PgLosses++;
            string id = Tech(c, lab).Id;
            Reset(lab);
            Events.Add(state, EventKind.ProgrammeEnded, lab.Id, 0, refund, id).Pick = 0;
        }

        static void Finish(GameConfig c, GameState state, LabState lab)
        {
            TechConfig tech = Tech(c, lab);
            long back = lab.PgInsight * MathI.MulPermille(tech.Cost, c.Programmes.InsightPermille);
            lab.ResearchPoints += back;
            lab.Prestige = (int)MathI.Min(c.Prestige.Max, Prestige.Level(c, lab) + (long)lab.PgInsight * c.Programmes.InsightPrestige);
            if (!LabQueries.HasTech(lab, tech.Id))
            {
                int lossBefore = ControlStale.LossNow(c, lab, state.Tick);
                lab.Techs.Add(tech.Id);
                Events.Add(state, EventKind.TechResearched, lab.Id, -1, 0, tech.Id);
                ControlStale.OnResearched(c, state, lab, lossBefore, tech);
            }
            lab.PgWins++;
            Reset(lab);
            Events.Add(state, EventKind.ProgrammeEnded, lab.Id, 0, back, tech.Id).Pick = 1;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || !Active(lab) || lab.PgPhase != Running) return;
            lab.PgLeft--;
            if (lab.PgLeft > 0) return;
            string id = Tech(c, lab).Id;
            bool bold = lab.PgBold == 1;
            if (bold && !state.Rng.Training.Chance(c.Programmes.BoldChancePermille))
            {
                lab.PgPhase = Failed;
                lab.PgLeft = 0;
                Events.Add(state, EventKind.ProgrammeStageFailed, lab.Id, 0, lab.PgDone + 1, id);
                return;
            }
            lab.PgDone++;
            if (bold) lab.PgInsight++;
            lab.PgBold = 0;
            Events.Add(state, EventKind.ProgrammeStageDone, lab.Id, 0, lab.PgDone, id).Pick = bold ? 2 : 1;
            if (lab.PgDone >= c.Programmes.Stages) Finish(c, state, lab);
            else lab.PgPhase = Choosing;
        }
    }
}
