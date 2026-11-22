using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>The training-data pipeline. See <see cref="DatasetsConfig"/>. Only the player's lab.</summary>
    public static class Datasets
    {
        public const int Scraped = 1;
        public const int Licensed = 2;
        public const int Synthetic = 3;
        public const int Users = 4;

        public static bool Enabled(GameConfig c) { return c.Datasets.Enabled > 0; }

        public static int Raw(LabState lab, int kind)
        {
            return kind == Scraped ? lab.DataRaw1 : kind == Licensed ? lab.DataRaw2 : kind == Synthetic ? lab.DataRaw3 : lab.DataRaw4;
        }

        public static int Clean(LabState lab, int kind)
        {
            return kind == Scraped ? lab.DataClean1 : kind == Licensed ? lab.DataClean2 : kind == Synthetic ? lab.DataClean3 : lab.DataClean4;
        }

        public static void AddRaw(LabState lab, int kind, int n)
        {
            if (kind == Scraped) lab.DataRaw1 += n; else if (kind == Licensed) lab.DataRaw2 += n; else if (kind == Synthetic) lab.DataRaw3 += n; else lab.DataRaw4 += n;
        }

        public static void AddClean(LabState lab, int kind, int n)
        {
            if (kind == Scraped) lab.DataClean1 += n; else if (kind == Licensed) lab.DataClean2 += n; else if (kind == Synthetic) lab.DataClean3 += n; else lab.DataClean4 += n;
        }

        public static int Stock(LabState lab)
        {
            return lab.DataRaw1 + lab.DataRaw2 + lab.DataRaw3 + lab.DataRaw4 + lab.DataClean1 + lab.DataClean2 + lab.DataClean3 + lab.DataClean4 + lab.DataJobUnits;
        }

        public static bool HasReleased(LabState lab)
        {
            for (int i = 0; i < lab.Models.Count; i++) if (lab.Models[i].Status == ModelStatus.Released) return true;
            return false;
        }

        public static int TotalClean(LabState lab) { return lab.DataClean1 + lab.DataClean2 + lab.DataClean3 + lab.DataClean4; }

        public static int Cost(GameConfig c, int kind) { return kind == Scraped ? c.Datasets.CostScrape : kind == Licensed ? c.Datasets.CostLicense : c.Datasets.CostSynth; }

        public static int Yield(GameConfig c, int kind)
        {
            return kind == Scraped ? c.Datasets.YieldScrape : kind == Licensed ? c.Datasets.YieldLicense : kind == Synthetic ? c.Datasets.YieldSynth : c.Datasets.YieldUsers;
        }

        public static int Quality(GameConfig c, int kind)
        {
            return kind == Scraped ? c.Datasets.QScrape : kind == Licensed ? c.Datasets.QLicense : kind == Synthetic ? c.Datasets.QSynth : c.Datasets.QUsers;
        }

        /// <summary>The clean corpus counted by what each source is worth.</summary>
        public static long Worth(GameConfig c, LabState lab)
        {
            long worth = 0;
            for (int k = 1; k <= 4; k++) worth += MathI.MulPermille(Clean(lab, k), Quality(c, k));
            return worth;
        }

        public static int SyntheticShare(LabState lab)
        {
            int total = TotalClean(lab);
            return total == 0 ? 0 : (int)((long)lab.DataClean3 * MathI.Permille / total);
        }

        /// <summary>True when too much of the clean corpus is synthetic.</summary>
        public static bool Collapsing(GameConfig c, LabState lab) { return SyntheticShare(lab) > c.Datasets.CollapseShare; }

        /// <summary>The capability gain, in permille of a line's base, that the clean corpus gives a new model.</summary>
        public static int BonusPermille(GameConfig c, LabState lab)
        {
            if (!Enabled(c)) return 0;
            long bonus = MathI.Min(c.Datasets.BonusCap, Worth(c, lab) * c.Datasets.BonusPer100 / 100);
            if (bonus <= 0) return 0;
            int over = SyntheticShare(lab) - c.Datasets.CollapseShare;
            if (over > 0) bonus -= MathI.MulPermille(bonus, MathI.Min(c.Datasets.CollapseCap, (long)over * c.Datasets.CollapseSlope));
            return (int)MathI.Max(0, bonus);
        }

        public static int ScrapedHeld(LabState lab)
        {
            return lab.DataRaw1 + lab.DataClean1 + (lab.DataJobKind == Scraped ? lab.DataJobUnits : 0);
        }

        public static void Acquire(GameConfig c, GameState state, LabState lab, int kind, int units)
        {
            long cost = (long)Cost(c, kind) * units;
            if (cost > 0) Finance.Spend(lab, LedgerLine.Training, cost);
            AddRaw(lab, kind, units);
            Events.Add(state, EventKind.DataAcquired, lab.Id, kind, units, "");
        }

        public static void StartClean(GameConfig c, LabState lab, int kind, int units)
        {
            AddRaw(lab, kind, -units);
            lab.DataJobKind = kind;
            lab.DataJobUnits = units;
            lab.DataJobLeft = c.Datasets.CleanWeeks;
            long cost = (long)c.Datasets.CleanCost * units;
            if (cost > 0) Finance.Spend(lab, LedgerLine.Training, cost);
        }

        public static void Backfill(GameConfig c, GameState state, LabState lab, int units)
        {
            long cost = (long)c.Datasets.BackfillCost * units;
            if (cost > 0) Finance.Spend(lab, LedgerLine.Training, cost);
            int left = units;
            int fromClean = (int)MathI.Min(left, lab.DataClean1);
            lab.DataClean1 -= fromClean;
            lab.DataClean2 += fromClean;
            left -= fromClean;
            lab.DataRaw1 -= left;
            lab.DataRaw2 += left;
            lab.DataBackfilled += units;
            Events.Add(state, EventKind.DataLicensed, lab.Id, 0, units, "");
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            if (lab.DataJobLeft > 0 && !(c.Datasets.WaveStallsCleaning > 0 && Spot.Enabled(c) && lab.SpotOut > 0))
            {
                lab.DataJobLeft--;
                if (lab.DataJobLeft == 0)
                {
                    int kind = lab.DataJobKind;
                    int good = (int)MathI.MulPermille(lab.DataJobUnits, Yield(c, kind));
                    AddClean(lab, kind, good);
                    lab.DataJobKind = 0;
                    lab.DataJobUnits = 0;
                    Events.Add(state, EventKind.DataCleaned, lab.Id, kind, good, "");
                }
            }
            if (state.Tick % c.Datasets.SuitWeeks != 0) return;
            if (c.Eras.IndexOf(EraSystem.AtTick(c, state.Tick)) < c.Datasets.SuitEra) return;
            int held = ScrapedHeld(lab);
            if (held <= 0) return;
            long fine = (long)held * c.Datasets.SuitFine;
            if (lab.GovDirectors > 0 && Governance.Enabled(c)) fine = MathI.MulPermille(fine, MathI.Max(0, MathI.Permille - (long)lab.GovDirectors * c.Datasets.DirectorSuitPermille));
            lab.DataRaw1 = 0;
            lab.DataClean1 = 0;
            if (lab.DataJobKind == Scraped)
            {
                lab.DataJobKind = 0;
                lab.DataJobUnits = 0;
                lab.DataJobLeft = 0;
            }
            lab.DataSuits++;
            if (fine > 0) Finance.Spend(lab, LedgerLine.Fines, fine);
            Events.Add(state, EventKind.DataSuit, lab.Id, held, fine, "");
        }
    }
}
