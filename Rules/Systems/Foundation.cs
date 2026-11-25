using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>The lab's endowed foundation. See <see cref="FoundationConfig"/>. Only the player's lab.</summary>
    public static class Foundation
    {
        public const int Safety = 1;
        public const int Education = 2;
        public const int OpenScience = 3;

        public static bool Enabled(GameConfig c) { return c.Foundation.Enabled > 0; }

        public static bool Exists(LabState lab) { return lab.FoundFund > 0; }

        public static int Payout(GameConfig c, int level) { return level == 1 ? c.Foundation.PayoutA : level == 2 ? c.Foundation.PayoutB : level == 3 ? c.Foundation.PayoutC : 0; }

        public static int Damping(GameConfig c, LabState lab)
        {
            if (!Enabled(c) || !Exists(lab) || lab.FoundFocus != Safety) return 0;
            return (int)MathI.Min(c.Foundation.MaxDamp, lab.FoundGoodwill * c.Foundation.SafetyDamp / MathI.Permille);
        }

        public static int ClaimStrength(GameConfig c, LabState lab)
        {
            if (!Enabled(c) || !Exists(lab) || lab.FoundFocus != OpenScience) return 0;
            return lab.FoundGoodwill * c.Foundation.ScienceStrength / MathI.Permille;
        }

        public static void Endow(GameConfig c, GameState state, LabState lab, long amount, int focus)
        {
            if (!Exists(lab))
            {
                lab.FoundFocus = focus;
                lab.FoundSwitch = state.Tick;
            }
            lab.FoundFund += amount;
            Finance.Spend(lab, LedgerLine.CapitalIn, amount);
            Events.Add(state, EventKind.FoundationEndowed, lab.Id, lab.FoundFocus, amount, "");
        }

        public static void Refocus(GameConfig c, GameState state, LabState lab, int focus)
        {
            lab.FoundFocus = focus;
            lab.FoundSwitch = state.Tick;
            lab.FoundGoodwill = (int)MathI.MulPermille(lab.FoundGoodwill, MathI.Permille - c.Foundation.SwitchLoss);
            Events.Add(state, EventKind.FoundationRefocused, lab.Id, -1, focus, "");
        }

        static void Clear(LabState lab)
        {
            lab.FoundFund = 0;
            lab.FoundGiven = 0;
            lab.FoundFocus = 0;
            lab.FoundPayout = 0;
            lab.FoundGoodwill = 0;
            lab.FoundSwitch = 0;
            lab.FoundDrip = 0;
        }

        public static long WindUpValue(GameConfig c, LabState lab) { return MathI.MulPermille(lab.FoundFund, c.Foundation.WindUpPermille); }

        public static void WindUp(GameConfig c, GameState state, LabState lab)
        {
            long back = WindUpValue(c, lab);
            Finance.Earn(lab, LedgerLine.CapitalIn, back);
            PressSystem.AddReputation(lab, -c.Foundation.WindUpReputation);
            Clear(lab);
            Events.Add(state, EventKind.FoundationWoundUp, lab.Id, -1, back, "");
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || !Exists(lab)) return;
            lab.FoundFund += MathI.MulPermille(lab.FoundFund, c.Foundation.ReturnPermille);
            int level = Payout(c, lab.FoundPayout);
            long give = level <= 0 ? 0 : MathI.Min(lab.FoundFund, MathI.Max(1, MathI.MulPermille(lab.FoundFund, level)));
            lab.FoundFund -= give;
            lab.FoundGiven += give;
            lab.FoundGoodwill = (int)MathI.Clamp(lab.FoundGoodwill - MathI.MulPermille(lab.FoundGoodwill, c.Foundation.GoodwillDecay) + give / c.Foundation.GoodwillUnit, 0, c.Foundation.MaxGoodwill);
            lab.FoundDrip++;
            if (lab.FoundFocus == Education && lab.FoundGoodwill >= c.Foundation.EducationAt && lab.FoundDrip % c.Foundation.EducationEvery == 0)
                PressSystem.AddReputation(lab, 1);
            if (lab.FoundFund <= 0)
            {
                long all = lab.FoundGiven;
                Clear(lab);
                Events.Add(state, EventKind.FoundationRanDry, lab.Id, -1, all, "");
                return;
            }
            if (lab.FoundDrip % c.Foundation.ReportWeeks == 0)
                Events.Add(state, EventKind.FoundationReported, lab.Id, lab.FoundGoodwill, lab.FoundGiven, "");
        }
    }
}
