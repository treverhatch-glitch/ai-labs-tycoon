using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>Preference and safety tuning of a trained model. See <see cref="PostTrainingConfig"/>. Only the player's lab.</summary>
    public static class PostTraining
    {
        public const int Preference = 1;
        public const int Safety = 2;

        public static bool Enabled(GameConfig c) { return c.PostTraining.Enabled > 0; }

        static long Step(long first, int decay, int n)
        {
            long v = first;
            for (int i = 0; i < n; i++) v = MathI.MulPermille(v, MathI.Permille - decay);
            return v;
        }

        /// <summary>What the next preference tuning of this model would add to its revenue.</summary>
        public static int NextPrefLift(GameConfig c, ModelState m) { return (int)Step(c.PostTraining.PrefLift, c.PostTraining.PrefDecay, m.PostPref); }

        /// <summary>What the next safety tuning of this model would add to its alignment.</summary>
        public static int NextSafetyGain(GameConfig c, ModelState m) { return (int)Step(c.PostTraining.SafetyGain, c.PostTraining.SafetyDecay, m.PostSafety); }

        /// <summary>What the model's tuning so far adds to its revenue, in permille (the preference runs less the refusals).</summary>
        public static int NetLift(GameConfig c, ModelState m)
        {
            long lift = 0;
            for (int i = 0; i < m.PostPref; i++) lift += Step(c.PostTraining.PrefLift, c.PostTraining.PrefDecay, i);
            lift -= (long)m.PostSafety * c.PostTraining.Refuse;
            return (int)MathI.Max(c.PostTraining.MinLift, lift);
        }

        public static int IncidentPermille(GameConfig c, ModelState m)
        {
            if (!Enabled(c)) return 0;
            long syco = (long)m.PostPref * c.PostTraining.Syco;
            if (Knowledge.Enabled(c)) syco = MathI.MulPermille(syco, MathI.Permille - MathI.Min(MathI.Permille, (long)m.DocLevel * c.PostTraining.DocsCutSyco));
            return (int)syco;
        }

        public static long Lift(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return 0;
            long total = 0;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.Status != ModelStatus.Released || (m.PostPref == 0 && m.PostSafety == 0)) continue;
                total += MathI.MulPermille(MarketSystem.ModelRevenue(state, lab.Id, m.Id), NetLift(c, m));
            }
            return total;
        }

        /// <summary>The clean units of the kinds a tuning of this kind uses.</summary>
        public static int Available(GameConfig c, LabState lab, int kind)
        {
            return kind == Preference ? lab.DataClean4 : lab.DataClean2 + lab.DataClean1;
        }

        static void Use(LabState lab, int kind, int units)
        {
            if (kind == Preference) { lab.DataClean4 -= units; return; }
            int fromLicensed = (int)MathI.Min(units, lab.DataClean2);
            lab.DataClean2 -= fromLicensed;
            lab.DataClean1 -= units - fromLicensed;
        }

        public static void Start(GameConfig c, GameState state, LabState lab, ModelState model, int kind)
        {
            Use(lab, kind, c.PostTraining.Units);
            if (c.PostTraining.Cost > 0) Finance.Spend(lab, LedgerLine.Training, c.PostTraining.Cost);
            model.PostKind = kind;
            model.PostLeft = c.PostTraining.Weeks;
            Events.Add(state, EventKind.PostTrainingStarted, lab.Id, model.Id, kind, model.Name);
        }

        public static void Abandon(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            model.PostKind = 0;
            model.PostLeft = 0;
            Events.Add(state, EventKind.PostTrainingAbandoned, lab.Id, model.Id, 0, model.Name);
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.PostKind == 0) continue;
                m.PostLeft--;
                if (m.PostLeft > 0) continue;
                int kind = m.PostKind;
                m.PostKind = 0;
                int runs;
                if (kind == Preference)
                {
                    m.PostPref++;
                    runs = m.PostPref;
                }
                else
                {
                    int gain = (int)Step(c.PostTraining.SafetyGain, c.PostTraining.SafetyDecay, m.PostSafety);
                    m.Alignment = (int)MathI.Min(MathI.Permille, m.Alignment + gain);
                    m.DisplayedAlignment = (int)MathI.Min(MathI.Permille, m.DisplayedAlignment + gain);
                    m.PostSafety++;
                    runs = m.PostSafety;
                }
                Events.Add(state, EventKind.PostTrainingDone, lab.Id, m.Id, runs, m.Name).Pick = kind == Preference ? 1 : 0;
            }
        }
    }
}
