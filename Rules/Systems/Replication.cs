using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>Other labs trying to reproduce the player's papers. See <see cref="ReplicationConfig"/>.</summary>
    public static class Replication
    {
        public static bool Enabled(GameConfig c) { return c.Replication.Enabled > 0; }

        public static bool Running(LabState lab) { return lab.ReplLeft > 0; }

        public static long EffortCost(GameConfig c, int effort) { return effort * (long)c.Replication.EffortPoints; }

        public static int Rigor(GameConfig c, LabState lab, int effort)
        {
            long writers = MathI.Min(10, Prestige.Writers(c, lab));
            return (int)MathI.Min(MathI.Permille, c.Replication.RigorBase + effort * (long)c.Replication.RigorStep + writers * c.Replication.RigorPerWriter);
        }

        /// <summary>How reproducible a paper has to be: more for a lab with a name to lose.</summary>
        public static int Bar(GameConfig c, LabState lab)
        {
            long prestige = Prestige.Level(c, lab);
            long atMax = c.Prestige.Max <= 0 ? 0 : prestige * c.Replication.BarPerPrestige / c.Prestige.Max;
            return (int)MathI.Min(MathI.Permille, c.Replication.BarBase + atMax);
        }

        static void LosePrestige(GameConfig c, LabState lab, int loss)
        {
            lab.Prestige = (int)MathI.Max(0, Prestige.Level(c, lab) - loss);
        }

        public static void Release(GameConfig c, GameState state, LabState lab, int effort)
        {
            long cost = EffortCost(c, effort);
            lab.ResearchPoints -= cost;
            lab.ReplRigor = Rigor(c, lab, effort);
            lab.ReplLeft = c.Replication.Weeks;
            Take(lab);
            Events.Add(state, EventKind.CodeReleased, lab.Id, lab.ReplRigor, cost, "").Pick = effort;
        }

        static void Take(LabState lab)
        {
            lab.ReplPending--;
            lab.ReplWait = 0;
        }

        public static void Withhold(GameConfig c, GameState state, LabState lab, int pick)
        {
            Take(lab);
            int before = Prestige.Level(c, lab);
            LosePrestige(c, lab, c.Replication.WithholdPrestige);
            Events.Add(state, EventKind.CodeWithheld, lab.Id, 0, before - Prestige.Level(c, lab), "").Pick = pick;
        }

        static void Resolve(GameConfig c, GameState state, LabState lab)
        {
            int rigor = lab.ReplRigor;
            bool holds = rigor >= Bar(c, lab);
            int before = Prestige.Level(c, lab);
            lab.ReplLeft = 0;
            lab.ReplRigor = 0;
            if (holds)
            {
                lab.ReplWins++;
                lab.Prestige = (int)MathI.Min(c.Prestige.Max, before + (long)c.Replication.WinPrestige);
                PressSystem.AddReputation(lab, c.Replication.WinReputation);
            }
            else
            {
                lab.ReplFails++;
                LosePrestige(c, lab, c.Replication.LossPrestige);
                PressSystem.AddReputation(lab, -PressSystem.Damp(c, lab, c.Replication.LossReputation));
            }
            Events.Add(state, EventKind.PaperReplicated, lab.Id, rigor, Prestige.Level(c, lab) - before, "").Pick = holds ? 1 : 0;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            int lastTick = state.Tick - 1;
            for (int i = state.Events.Count - 1; i >= 0; i--)
            {
                GameEvent e = state.Events[i];
                if (e.Tick < lastTick) break;
                if (e.Tick != lastTick || e.LabId != lab.Id || e.Pick != 1) continue;
                if ((e.Kind == EventKind.PaperPublished || e.Kind == EventKind.PaperDecided) && lab.ReplPending < c.Replication.PendingMax) lab.ReplPending++;
            }
            if (lab.ReplPending > 0)
            {
                lab.ReplWait++;
                if (lab.ReplWait > c.Replication.PendingWeeks) Withhold(c, state, lab, 1);
            }
            if (Running(lab))
            {
                lab.ReplLeft--;
                if (lab.ReplLeft <= 0) Resolve(c, state, lab);
            }
        }
    }
}
