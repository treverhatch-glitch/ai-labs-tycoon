using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>Incident tabletop drills. See <see cref="DrillsConfig"/>. Only the player lab.</summary>
    public static class Drills
    {
        public static bool Enabled(GameConfig c) { return c.Drills.Enabled > 0; }

        public static int IncidentCutPermille(GameConfig c, LabState lab)
        {
            if (!Enabled(c) || lab.DrillPrep <= 0) return 0;
            return (int)MathI.MulPermille(c.Drills.IncidentCut, lab.DrillPrep);
        }

        public static int ReputationCutPermille(GameConfig c, LabState lab)
        {
            if (!Enabled(c) || lab.DrillPrep <= 0) return 0;
            return (int)MathI.MulPermille(c.Drills.RepCut, lab.DrillPrep);
        }

        static long Mix(GameState state, LabState lab)
        {
            long mix = (state.Seed * 47 + state.Tick) * 47 + lab.Id * 31 + 17;
            mix = (mix & 0x7FFFFFFFL) * 2654435761L & 0xFFFFFFFFL;
            return mix >> 9;
        }

        public static void Start(GameConfig c, GameState state, LabState lab)
        {
            if (c.Drills.Cost > 0) Finance.Spend(lab, LedgerLine.Training, c.Drills.Cost);
            lab.DrillLeft = c.Drills.Weeks;
            Events.Add(state, EventKind.DrillStarted, lab.Id, -1, c.Drills.Weeks, "");
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            int now = state.Tick + 1;
            if (lab.DrillLeft > 0)
            {
                lab.DrillLeft--;
                if (lab.DrillLeft > 0) return;
                bool pass = (int)(Mix(state, lab) % MathI.Permille) < MathI.Min(MathI.Permille, lab.DrillPrep + 400);
                if (pass)
                {
                    lab.DrillPassed++;
                    lab.DrillPrep = (int)MathI.Min(MathI.Permille, lab.DrillPrep + c.Drills.PassGain);
                }
                else
                {
                    lab.DrillFailed++;
                    lab.DrillPrep = (int)MathI.Max(0, lab.DrillPrep - c.Drills.FailLoss);
                }
                lab.DrillDue = now + c.Drills.Interval;
                GameEvent e = Events.Add(state, EventKind.DrillFinished, lab.Id, -1, lab.DrillPrep, "");
                e.Pick = pass ? 1 : 0;
                return;
            }
            int decay = c.Drills.Decay;
            if (lab.DrillDue > 0 && now > lab.DrillDue) decay += c.Drills.IdleDecay;
            if (decay > 0 && lab.DrillPrep > 0) lab.DrillPrep = (int)MathI.Max(0, lab.DrillPrep - decay);
        }
    }
}
