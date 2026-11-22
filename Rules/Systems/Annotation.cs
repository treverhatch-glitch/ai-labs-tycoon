using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>Annotation vendors for the data pipeline. See <see cref="AnnotationConfig"/>. Only the player's lab.</summary>
    public static class Annotation
    {
        public static bool Enabled(GameConfig c) { return c.Annotation.Enabled > 0; }

        public static int Weekly(GameConfig c, int v) { return v == 1 ? c.Annotation.Weekly1 : v == 2 ? c.Annotation.Weekly2 : v == 3 ? c.Annotation.Weekly3 : 0; }
        public static int BonusOf(GameConfig c, int v) { return v == 1 ? c.Annotation.Bonus1 : v == 2 ? c.Annotation.Bonus2 : v == 3 ? c.Annotation.Bonus3 : 0; }
        public static int Cap(GameConfig c, int v) { return v == 1 ? c.Annotation.Cap1 : v == 2 ? c.Annotation.Cap2 : v == 3 ? c.Annotation.Cap3 : 0; }
        public static int Risk(GameConfig c, int v) { return v == 1 ? c.Annotation.Risk1 : v == 2 ? c.Annotation.Risk2 : v == 3 ? c.Annotation.Risk3 : 0; }

        /// <summary>What the vendor adds to the yield of a cleaning job of this size.</summary>
        public static int Bonus(GameConfig c, LabState lab, int units)
        {
            if (!Enabled(c) || lab.AnnVendor == 0 || units > Cap(c, lab.AnnVendor)) return 0;
            return BonusOf(c, lab.AnnVendor);
        }

        /// <summary>The weekly chance of a scandal from what has built up.</summary>
        public static int ScandalChance(GameConfig c, LabState lab) { return lab.AnnExposure / c.Annotation.ExposeDiv; }

        public static long EarlyFee(GameConfig c, LabState lab)
        {
            return MathI.MulPermille((long)lab.AnnLeft * Weekly(c, lab.AnnVendor), c.Annotation.EarlyPermille);
        }

        public static void Contract(GameConfig c, GameState state, LabState lab, int vendor)
        {
            lab.AnnVendor = vendor;
            lab.AnnLeft = c.Annotation.Term;
            Events.Add(state, EventKind.AnnotatorsContracted, lab.Id, vendor, 0, "");
        }

        static void End(GameConfig c, GameState state, LabState lab, int pick)
        {
            int vendor = lab.AnnVendor;
            lab.AnnVendor = 0;
            lab.AnnLeft = 0;
            Events.Add(state, EventKind.AnnotatorsEnded, lab.Id, vendor, 0, "").Pick = pick;
        }

        public static void EndEarly(GameConfig c, GameState state, LabState lab)
        {
            long fee = EarlyFee(c, lab);
            if (fee > 0) Finance.Spend(lab, LedgerLine.Training, fee);
            End(c, state, lab, 0);
        }

        public static void Audit(GameConfig c, GameState state, LabState lab)
        {
            if (c.Annotation.AuditCost > 0) Finance.Spend(lab, LedgerLine.Training, c.Annotation.AuditCost);
            lab.AnnExposure -= (int)MathI.MulPermille(lab.AnnExposure, c.Annotation.AuditCut);
            lab.AnnAuditWeek = state.Tick + 1;
            Events.Add(state, EventKind.VendorAudited, lab.Id, lab.AnnVendor, lab.AnnExposure, "");
        }

        static long Mix(GameState state, LabState lab)
        {
            long mix = (state.Seed * 43 + state.Tick) * 43 + lab.Id * 29 + 11;
            mix = (mix & 0x7FFFFFFFL) * 2654435761L & 0xFFFFFFFFL;
            return mix >> 8;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || lab.AnnVendor == 0) return;
            long fee = Weekly(c, lab.AnnVendor);
            if (fee > 0) Finance.Spend(lab, LedgerLine.Training, MathI.Min(fee, MathI.Max(0, lab.Cash)));
            lab.AnnExposure = (int)MathI.Min(MathI.Permille, lab.AnnExposure + Risk(c, lab.AnnVendor));
            int chance = ScandalChance(c, lab);
            if (chance > 0 && (int)(Mix(state, lab) % MathI.Permille) < chance)
            {
                int vendor = lab.AnnVendor;
                lab.AnnStrikes++;
                lab.AnnExposure = c.Annotation.ExposeReset;
                if (c.Annotation.ExposeFine > 0) Finance.Spend(lab, LedgerLine.Fines, c.Annotation.ExposeFine);
                for (int i = 0; i < lab.Models.Count; i++)
                {
                    ModelState m = lab.Models[i];
                    if (m.Status != ModelStatus.Released) continue;
                    for (int s = 0; s < m.Sectors.Count; s++) TrustSystem.Shift(c, state, lab, m.Sectors[s], -c.Annotation.ExposeTrust, false);
                }
                Events.Add(state, EventKind.VendorScandal, lab.Id, vendor, c.Annotation.ExposeFine, "");
                End(c, state, lab, 2);
                return;
            }
            lab.AnnLeft--;
            if (lab.AnnLeft <= 0) End(c, state, lab, 1);
        }
    }
}
