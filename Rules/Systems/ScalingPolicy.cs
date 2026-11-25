using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>The scaling policy. See <see cref="ScalingConfig"/>. Only the player's lab keeps one.</summary>
    public static class ScalingPolicy
    {
        public static bool Enabled(GameConfig c) { return c.Scaling.Enabled > 0; }

        public static int Tripwire(GameConfig c, int level) { return c.Scaling.FirstTripwire + (level - 1) * c.Scaling.TripwireStep; }

        public static int Safety(LabState lab) { return LabQueries.StaffCount(lab, "safety"); }

        /// <summary>True when the lab holds the safeguards for the level: a security tier and safety people for each level.</summary>
        public static bool Safeguarded(GameConfig c, LabState lab, int level)
        {
            return lab.SecurityTier >= level && Safety(lab) + Standards.SafetyRelief(c, lab) >= level * c.Scaling.SafetyPerTier;
        }

        /// <summary>The highest tripwire of the lab's policy a trained model has reached without its safeguards, or 0. When the model is wanted it is returned too.</summary>
        public static int Tripped(GameConfig c, LabState lab, out ModelState model)
        {
            model = null;
            if (!Enabled(c)) return 0;
            int worst = 0;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.Status != ModelStatus.Trained) continue;
                for (int k = lab.PolicyLevel; k > worst; k--)
                {
                    if (m.Capability < Tripwire(c, k) || Safeguarded(c, lab, k)) continue;
                    worst = k;
                    model = m;
                    break;
                }
            }
            return worst;
        }

        public static int Tripped(GameConfig c, LabState lab)
        {
            ModelState ignored;
            return Tripped(c, lab, out ignored);
        }

        /// <summary>True when the trained model is held back by a tripwire the lab has no safeguards for.</summary>
        public static bool Holds(GameConfig c, LabState lab, ModelState model)
        {
            if (!Enabled(c) || model.Status != ModelStatus.Trained) return false;
            for (int k = lab.PolicyLevel; k >= 1; k--)
                if (model.Capability >= Tripwire(c, k) && !Safeguarded(c, lab, k)) return true;
            return false;
        }

        public static int DampPermille(GameConfig c, LabState lab) { return Enabled(c) ? lab.PolicyLevel * c.Scaling.DampPermille : 0; }

        public static int ClaimStrength(GameConfig c, LabState lab) { return Enabled(c) ? lab.PolicyLevel * c.Scaling.ClaimStrength : 0; }

        public static long RaiseFee(GameConfig c, LabState lab, int level) { return c.Scaling.AdoptFee * (level - lab.PolicyLevel); }

        /// <summary>The lowest tripwire of the policy any trained model has reached without its safeguards, or 0.</summary>
        public static int Lowest(GameConfig c, LabState lab)
        {
            int lowest = 0;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.Status != ModelStatus.Trained) continue;
                for (int k = 1; k <= lab.PolicyLevel; k++)
                {
                    if (m.Capability < Tripwire(c, k) || Safeguarded(c, lab, k)) continue;
                    if (lowest == 0 || k < lowest) lowest = k;
                    break;
                }
            }
            return lowest;
        }

        static void ClearAbove(GameState state, LabState lab)
        {
            if (lab.PolicyHold <= lab.PolicyLevel) return;
            Events.Add(state, EventKind.TripwireCleared, lab.Id, -1, lab.PolicyHold, "").Pick = 0;
            lab.PolicyHold = lab.PolicyLevel;
        }

        public static void Set(GameConfig c, GameState state, LabState lab, int level)
        {
            int before = lab.PolicyLevel;
            bool raise = level > before;
            if (raise) Finance.Spend(lab, LedgerLine.Fines, RaiseFee(c, lab, level));
            else lab.Reputation = (int)MathI.Max(0, lab.Reputation - PressSystem.Damp(c, lab, c.Scaling.WalkBackReputation));
            lab.PolicyLevel = level;
            ClearAbove(state, lab);
            lab.PolicyLockedUntil = state.Tick + c.Scaling.LockWeeks;
            Events.Add(state, EventKind.PolicyChanged, lab.Id, -1, level, raise ? "raise" : "lower").Pick = before;
        }

        public static void Break(GameConfig c, GameState state, LabState lab)
        {
            int before = lab.PolicyLevel;
            lab.PolicyLevel = Lowest(c, lab) - 1;
            ClearAbove(state, lab);
            lab.PolicyBreaks++;
            lab.PolicyLockedUntil = state.Tick + c.Scaling.LockWeeks;
            lab.Reputation = (int)MathI.Max(0, lab.Reputation - PressSystem.Damp(c, lab, c.Scaling.OverrideReputation));
            for (int i = 0; i < lab.Staff.Count; i++)
                if (lab.Staff[i].RoleId == "safety") lab.Staff[i].Morale = (int)MathI.Max(0, lab.Staff[i].Morale - c.Scaling.OverrideMorale);
            Events.Add(state, EventKind.PolicyChanged, lab.Id, -1, lab.PolicyLevel, "break").Pick = before;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            ModelState model;
            int now = Tripped(c, lab, out model);
            if (now == lab.PolicyHold) return;
            if (now > lab.PolicyHold && model != null)
                Events.Add(state, EventKind.TripwireHit, lab.Id, model.Id, now, model.Name);
            else if (now == 0)
                Events.Add(state, EventKind.TripwireCleared, lab.Id, -1, lab.PolicyHold, "").Pick = lab.PolicyHold > 0 && Safeguarded(c, lab, lab.PolicyHold) ? 1 : 0;
            lab.PolicyHold = now;
        }
    }
}
