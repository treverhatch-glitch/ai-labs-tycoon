using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>The lab's name as an employer. See <see cref="BrandConfig"/>. Only the player's lab.</summary>
    public static class Brand
    {
        public static bool Enabled(GameConfig c) { return c.Brand.Enabled > 0; }

        public static bool On(LabState lab) { return lab.BrandOn > 0; }

        public static bool Waiting(LabState lab) { return lab.BrandReview > 0; }

        public static long RepairCost(GameConfig c, LabState lab) { return lab.Staff.Count * (long)c.Brand.RepairCostPerStaff; }

        /// <summary>The change to every hire fee in permille: negative with a good name, positive with a bad one. Zero without a careers page.</summary>
        public static int HireFeePermille(GameConfig c, LabState lab)
        {
            if (!Enabled(c) || !On(lab)) return 0;
            return (int)(-((lab.BrandScore - 500L) * c.Brand.MaxFeePermille / 500));
        }

        static void Move(LabState lab, long points)
        {
            lab.BrandScore = (int)MathI.Clamp(lab.BrandScore + points, 0, 1000);
        }

        public static void Open(GameConfig c, GameState state, LabState lab)
        {
            Finance.Spend(lab, LedgerLine.Hiring, c.Brand.OpenCost);
            lab.BrandOn = 1;
            lab.BrandScore = c.Brand.Start;
            lab.BrandLow = lab.BrandScore < c.Brand.LowBar ? 1 : 0;
            Events.Add(state, EventKind.CareersOpened, lab.Id, 0, c.Brand.OpenCost, "");
        }

        public static void Answer(GameConfig c, GameState state, LabState lab, int stance)
        {
            long before = lab.BrandScore;
            if (stance == 1)
            {
                Finance.Spend(lab, LedgerLine.Hiring, RepairCost(c, lab));
                Move(lab, c.Brand.RepairPoints);
            }
            else if (stance == 2)
                PressSystem.AddReputation(lab, -PressSystem.Damp(c, lab, c.Brand.RebutReputation));
            else Move(lab, -(long)c.Brand.IgnorePoints);
            Finish(c, state, lab, stance, lab.BrandScore - before);
        }

        static void Finish(GameConfig c, GameState state, LabState lab, int pick, long change)
        {
            lab.BrandReview = 0;
            if (lab.BrandScore >= c.Brand.LowBar) lab.BrandLow = 0;
            Events.Add(state, EventKind.ReviewAnswered, lab.Id, 0, change, "").Pick = pick;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || !On(lab)) return;
            if (Waiting(lab))
            {
                lab.BrandReview--;
                if (lab.BrandReview == 0)
                {
                    long before = lab.BrandScore;
                    Move(lab, -(long)c.Brand.IgnorePoints);
                    Finish(c, state, lab, 0, lab.BrandScore - before);
                }
            }
            int lastTick = state.Tick - 1;
            for (int i = state.Events.Count - 1; i >= 0; i--)
            {
                GameEvent e = state.Events[i];
                if (e.Tick < lastTick) break;
                if (e.Tick != lastTick || e.LabId != lab.Id) continue;
                switch (e.Kind)
                {
                    case EventKind.StaffPromoted:
                    case EventKind.EquityGranted:
                    case EventKind.MentoringStarted:
                    case EventKind.SabbaticalStarted:
                    case EventKind.InternshipStarted:
                        Move(lab, c.Brand.KeptPoints);
                        break;
                    case EventKind.LayoffRound:
                    case EventKind.StaffQuit:
                    case EventKind.LabourStrike:
                        Move(lab, -(long)c.Brand.BrokenPoints);
                        break;
                }
            }
            if (lab.BrandScore < c.Brand.LowBar && lab.BrandLow == 0)
            {
                lab.BrandLow = 1;
                lab.BrandReview = c.Brand.ReviewWeeks;
                lab.BrandReviews++;
                Events.Add(state, EventKind.BadReview, lab.Id, 0, lab.BrandScore, "");
            }
            else if (lab.BrandScore >= c.Brand.LowBar && !Waiting(lab)) lab.BrandLow = 0;
        }
    }
}
