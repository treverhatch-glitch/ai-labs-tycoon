using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>The world events that fall on the player's lab. See <see cref="ShocksConfig"/>.</summary>
    public static class Shocks
    {
        public const int Squeeze = 1;
        public const int Backlash = 2;
        public const int Boom = 3;

        public static bool Enabled(GameConfig c) { return c.Shocks.Enabled > 0; }

        public static bool Pending(LabState lab) { return lab.ShockKind > 0; }

        /// <summary>Which kind of shock a world event is, or 0 if it is only news.</summary>
        public static int KindOf(string id)
        {
            switch (id)
            {
                case "talent-war":
                case "friday-firing":
                case "card-shortage":
                case "sold-out-chip":
                case "heatwave":
                case "everyone-home":
                    return Squeeze;
                case "teen-chatbot":
                case "first-crash":
                case "too-dangerous":
                case "pause-letter":
                case "invented-cases":
                case "deleted-tests":
                case "polite-flash-crash":
                case "fridge-recall":
                case "bank-wobble":
                    return Backlash;
                case "glass-rectangle":
                case "phone-assistant":
                case "quiz-show":
                case "contest-winner":
                case "attention-paper":
                case "code-autocomplete":
                case "hundred-million":
                case "cheap-challenger":
                case "work-as-a-service":
                case "co-author":
                    return Boom;
                default:
                    return 0;
            }
        }

        public static bool CanCounter(GameConfig c, LabState lab)
        {
            ModelState best = LabQueries.BestReleased(lab);
            return best != null && best.Capability >= c.Shocks.CounterBar;
        }

        public static long BraceCost(GameConfig c, LabState lab) { return lab.Staff.Count * (long)c.Shocks.BraceCost; }

        static void Shift(LabState lab, int delta)
        {
            for (int i = 0; i < lab.Staff.Count; i++) lab.Staff[i].Morale = (int)MathI.Clamp((long)lab.Staff[i].Morale + delta, 0, MathI.Permille);
        }

        /// <summary>The hit of a squeeze or a backlash at this share of its full size. Returns the reputation it cost.</summary>
        static int Hit(GameConfig c, LabState lab, int kind, long permille)
        {
            if (kind == Squeeze)
            {
                Finance.Spend(lab, LedgerLine.Hiring, MathI.MulPermille(lab.Staff.Count * (long)c.Shocks.HitCash, permille));
                Shift(lab, -(int)MathI.MulPermille(c.Shocks.HitMorale, permille));
                return 0;
            }
            int loss = PressSystem.Damp(c, lab, (int)MathI.MulPermille(MathI.MulPermille(c.Shocks.HitReputation, permille), Creed.Exposure(c, lab)));
            PressSystem.AddReputation(lab, -loss);
            return -loss;
        }

        public static void OnWorldEvent(GameConfig c, GameState state, int index)
        {
            if (!Enabled(c) || state.PlayerLabId < 0) return;
            int kind = KindOf(c.WorldEvents[index].Id);
            if (kind == 0) return;
            LabState lab = state.Lab(state.PlayerLabId);
            if (lab == null) return;
            if (Pending(lab)) Resolve(c, state, lab, 2, 5);
            lab.ShockKind = kind;
            lab.ShockRef = index + 1;
            lab.ShockLeft = c.Shocks.WindowWeeks;
            Events.Add(state, EventKind.ShockStruck, lab.Id, kind, 0, c.WorldEvents[index].Id);
        }

        /// <summary>Answers the waiting shock. Stance 1 brace, 2 ride, 3 counter. Pick is what the event records.</summary>
        public static void Resolve(GameConfig c, GameState state, LabState lab, int stance, int lapsePick)
        {
            int kind = lab.ShockKind;
            string id = c.WorldEvents[lab.ShockRef - 1].Id;
            int pick = lapsePick > 0 ? lapsePick : stance;
            int rep = 0;
            long boom = c.Shocks.BoomReputation;
            if (stance == 1)
            {
                Finance.Spend(lab, LedgerLine.Hiring, BraceCost(c, lab));
                lab.ShockBraced++;
                if (kind == Boom) rep = (int)MathI.MulPermille(boom, c.Shocks.BoomBrace);
                else rep = Hit(c, lab, kind, c.Shocks.BracePermille);
            }
            else if (stance == 2)
            {
                lab.ShockRode++;
                if (kind == Boom) rep = (int)boom;
                else rep = Hit(c, lab, kind, MathI.Permille);
            }
            else
            {
                bool works = CanCounter(c, lab);
                lab.ShockCountered++;
                pick = works ? 3 : 4;
                if (kind == Boom) rep = works ? (int)MathI.MulPermille(boom, c.Shocks.CounterBoom) : -(int)MathI.MulPermille(boom, c.Shocks.CounterMiss);
                else if (works) rep = c.Shocks.CounterGain;
                else rep = Hit(c, lab, kind, c.Shocks.CounterMiss);
            }
            if (kind == Boom || (stance == 3 && pick == 3))
            {
                if (rep < 0) rep = -PressSystem.Damp(c, lab, -rep);
                PressSystem.AddReputation(lab, rep);
            }
            lab.ShockKind = 0;
            lab.ShockRef = 0;
            lab.ShockLeft = 0;
            Events.Add(state, EventKind.ShockAnswered, lab.Id, kind, rep, id).Pick = pick;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || !Pending(lab)) return;
            lab.ShockLeft--;
            if (lab.ShockLeft <= 0) Resolve(c, state, lab, 2, 5);
        }
    }
}
