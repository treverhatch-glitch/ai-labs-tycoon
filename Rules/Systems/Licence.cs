using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>Licensing regimes that open with the eras. See <see cref="LicenceConfig"/>. Only the player's lab.</summary>
    public static class Licence
    {
        public const int Review = 1;
        public const int Licensed = 2;

        public static bool Enabled(GameConfig c) { return c.Licence.Enabled > 0; }

        public static bool Active(LabState lab) { return lab.LicRegime > 0; }

        public static bool Held(LabState lab) { return lab.LicPhase == Licensed; }

        public static int EraOf(GameConfig c, int regime) { return regime == 1 ? c.Licence.EraA : regime == 2 ? c.Licence.EraB : c.Licence.EraC; }

        public static bool Open(GameConfig c, int regime, int tick) { return c.Eras.IndexOf(EraSystem.AtTick(c, tick)) >= EraOf(c, regime); }

        public static long Fee(GameConfig c, int regime) { return regime == 1 ? c.Licence.FeeA : regime == 2 ? c.Licence.FeeB : c.Licence.FeeC; }

        public static int Term(GameConfig c, int regime) { return regime == 1 ? c.Licence.TermA : regime == 2 ? c.Licence.TermB : c.Licence.TermC; }

        public static int Bar(GameConfig c, int regime) { return regime == 1 ? c.Licence.BarA : regime == 2 ? c.Licence.BarB : c.Licence.BarC; }

        public static int Cut(GameConfig c, int regime) { return regime == 1 ? c.Licence.CutA : regime == 2 ? c.Licence.CutB : c.Licence.CutC; }

        public static bool MeetsBar(GameConfig c, LabState lab, int regime)
        {
            ModelState best = LabQueries.BestReleased(lab);
            return best != null && best.Alignment >= Bar(c, regime);
        }

        /// <summary>The share a held licence takes off an incident fine, in permille.</summary>
        public static int FineCutPermille(GameConfig c, LabState lab)
        {
            return Enabled(c) && Held(lab) ? Cut(c, lab.LicRegime) : 0;
        }

        public static void Apply(GameConfig c, GameState state, LabState lab, int regime)
        {
            Finance.Spend(lab, LedgerLine.Fines, Fee(c, regime));
            lab.LicRegime = regime;
            lab.LicPhase = Review;
            lab.LicLeft = c.Licence.ReviewWeeks;
            lab.LicStrikes = 0;
            Events.Add(state, EventKind.LicenceApplied, lab.Id, regime, Fee(c, regime), "").Pick = regime;
        }

        public static void Renew(GameConfig c, GameState state, LabState lab)
        {
            Finance.Spend(lab, LedgerLine.Fines, Fee(c, lab.LicRegime));
            lab.LicLeft = Term(c, lab.LicRegime);
            lab.LicStrikes = 0;
        }

        static void End(GameConfig c, GameState state, LabState lab, int pick)
        {
            int regime = lab.LicRegime;
            lab.LicRegime = 0;
            lab.LicPhase = 0;
            lab.LicLeft = 0;
            lab.LicStrikes = 0;
            Events.Add(state, EventKind.LicenceEnded, lab.Id, regime, 0, "").Pick = pick;
        }

        public static void Surrender(GameConfig c, GameState state, LabState lab) { End(c, state, lab, 1); }

        /// <summary>An incident on a model of a licensed lab: a strike, and at the limit the licence is suspended with a penalty.</summary>
        public static void OnIncident(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || !Held(lab)) return;
            lab.LicStrikes++;
            Events.Add(state, EventKind.LicenceStrike, lab.Id, lab.LicRegime, lab.LicStrikes, "");
            if (lab.LicStrikes < c.Licence.StrikeLimit) return;
            Finance.Spend(lab, LedgerLine.Fines, 2 * Fee(c, lab.LicRegime));
            PressSystem.AddReputation(lab, -PressSystem.Damp(c, lab, c.Licence.SuspendReputation));
            lab.LicSuspended++;
            lab.LicCooldown = state.Tick + c.Licence.CooldownWeeks;
            End(c, state, lab, 2);
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || !Active(lab)) return;
            lab.LicLeft--;
            if (lab.LicLeft > 0) return;
            if (lab.LicPhase == Review)
            {
                int regime = lab.LicRegime;
                bool granted = MeetsBar(c, lab, regime);
                Events.Add(state, EventKind.LicenceDecided, lab.Id, regime, 0, "").Pick = granted ? 1 : 0;
                if (granted)
                {
                    lab.LicPhase = Licensed;
                    lab.LicLeft = Term(c, regime);
                    lab.LicGranted++;
                }
                else
                {
                    lab.LicRefused++;
                    lab.LicCooldown = state.Tick + c.Licence.CooldownWeeks;
                    lab.LicRegime = 0;
                    lab.LicPhase = 0;
                    lab.LicLeft = 0;
                }
                return;
            }
            End(c, state, lab, 0);
        }
    }
}
