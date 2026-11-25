using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>Corporate tax and the tax audit. See <see cref="TaxConfig"/>. Only the player's lab pays.</summary>
    public static class Tax
    {
        public const int Cooperate = 0;
        public const int Settle = 1;
        public const int Contest = 2;
        public const int Lapsed = 3;

        public static bool Enabled(GameConfig c) { return c.Tax.Enabled > 0; }

        public static bool Auditing(LabState lab) { return lab.TaxAuditDue > 0; }

        public static int Relief(GameConfig c, int tier) { return tier == 2 ? c.Tax.OffshoreRelief : tier == 1 ? c.Tax.HoldingRelief : 0; }

        public static int ExposurePerWeek(GameConfig c, int tier) { return tier == 2 ? c.Tax.OffshoreExposure : tier == 1 ? c.Tax.HoldingExposure : 0; }

        public static long BuildPrice(GameConfig c, int tier) { return tier == 2 ? c.Tax.OffshoreCost : c.Tax.HoldingCost; }

        public static int BuildWeeks(GameConfig c, int tier) { return tier == 2 ? c.Tax.OffshoreWeeks : c.Tax.HoldingWeeks; }

        public static long WeeklyFee(GameConfig c, LabState lab) { return c.Tax.FeePerTier * lab.TaxStructure; }

        public static long CreditNow(GameConfig c, LabState lab) { return MathI.MulPermille(lab.TaxResearch, c.Tax.CreditPermille); }

        /// <summary>What an audit is about: what the structure saved and the credits claimed since the last one.</summary>
        public static long AtStake(LabState lab) { return lab.TaxSaved + lab.TaxClaimed; }

        public static long CooperateCost(GameConfig c, LabState lab) { return lab.TaxAuditOwed + MathI.MulPermille(lab.TaxAuditOwed, c.Tax.CooperateInterest); }

        public static long SettleCost(GameConfig c, LabState lab) { return MathI.MulPermille(lab.TaxAuditOwed, c.Tax.SettlePermille); }

        public static long LapseCost(GameConfig c, LabState lab) { return CooperateCost(c, lab) + MathI.MulPermille(lab.TaxAuditOwed, c.Tax.LapsePermille); }

        public static void Build(GameConfig c, GameState state, LabState lab, int tier)
        {
            if (tier == 0)
            {
                lab.TaxStructure = 0;
                lab.TaxBuilding = 0;
                lab.TaxDue = 0;
                return;
            }
            Finance.Spend(lab, LedgerLine.Fines, BuildPrice(c, tier));
            lab.TaxBuilding = tier;
            lab.TaxDue = state.Tick + BuildWeeks(c, tier);
        }

        public static void Claim(GameConfig c, GameState state, LabState lab)
        {
            long credit = CreditNow(c, lab);
            lab.TaxCredit += credit;
            lab.TaxClaimed += credit;
            lab.TaxResearch = 0;
            lab.TaxExposure += c.Tax.ClaimExposure;
            Events.Add(state, EventKind.TaxCreditClaimed, lab.Id, -1, credit, "");
        }

        static void Close(GameConfig c, GameState state, LabState lab, long paid, int choice)
        {
            lab.TaxAuditDue = 0;
            lab.TaxAuditOwed = 0;
            lab.TaxSaved = 0;
            lab.TaxClaimed = 0;
            Events.Add(state, EventKind.TaxAuditClosed, lab.Id, -1, paid, "").Pick = choice;
        }

        public static void Answer(GameConfig c, GameState state, LabState lab, int choice)
        {
            long owed = lab.TaxAuditOwed;
            if (choice == Cooperate)
            {
                long cost = CooperateCost(c, lab);
                Finance.Spend(lab, LedgerLine.Fines, cost);
                lab.TaxStructure = 0;
                lab.TaxBuilding = 0;
                lab.TaxDue = 0;
                lab.TaxExposure = 0;
                Close(c, state, lab, cost, Cooperate);
            }
            else if (choice == Settle)
            {
                long cost = SettleCost(c, lab);
                Finance.Spend(lab, LedgerLine.Fines, cost);
                lab.TaxExposure /= 2;
                Close(c, state, lab, cost, Settle);
            }
            else
            {
                int strength = c.Tax.ContestStrength + lab.TaxStructure * c.Tax.StructureStrength;
                lab.TaxExposure = 0;
                Close(c, state, lab, 0, Contest);
                Litigation.File(c, state, lab, Litigation.Tax, MathI.MulPermille(owed, c.Tax.ContestClaimPermille), strength, "");
            }
        }

        static void Pay(GameConfig c, GameState state, LabState lab)
        {
            long used = MathI.Min(lab.TaxCredit, lab.TaxAccrued);
            long pay = lab.TaxAccrued - used;
            lab.TaxCredit -= used;
            lab.TaxAccrued = 0;
            lab.TaxResearch = 0;
            if (pay > 0) Finance.Spend(lab, LedgerLine.Fines, pay);
            Events.Add(state, EventKind.TaxPaid, lab.Id, -1, pay, "").Pick = lab.TaxStructure > 0 ? 1 : 0;
            if (Auditing(lab) || lab.TaxExposure < c.Tax.AuditThreshold || AtStake(lab) <= 0) return;
            lab.TaxAuditDue = state.Tick + 1 + c.Tax.AuditWeeks;
            lab.TaxAuditOwed = AtStake(lab);
            if (lab.TaxStructure == 2) lab.Reputation = (int)MathI.Max(0, lab.Reputation - PressSystem.Damp(c, lab, c.Tax.ExposedReputation));
            Events.Add(state, EventKind.TaxAuditOpened, lab.Id, -1, lab.TaxAuditOwed, "").Pick = lab.TaxStructure == 2 ? 1 : 0;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            if (lab.TaxStructure > 0) Finance.Spend(lab, LedgerLine.Fines, WeeklyFee(c, lab));
            if (lab.TaxBuilding > 0 && state.Tick + 1 >= lab.TaxDue)
            {
                lab.TaxStructure = lab.TaxBuilding;
                lab.TaxBuilding = 0;
                lab.TaxDue = 0;
                Events.Add(state, EventKind.TaxStructureBuilt, lab.Id, -1, lab.TaxStructure, "");
            }
            long taxable = lab.Ledger.LastWeek.Net - lab.Ledger.LastWeek.Fines - lab.Ledger.LastWeek.CapitalIn - c.Tax.AllowancePerWeek + CapitalAllowances.Adjust(c, state, lab);
            if (taxable > 0)
            {
                long gross = MathI.MulPermille(taxable, c.Tax.RatePermille);
                long relief = MathI.MulPermille(gross, Relief(c, lab.TaxStructure));
                lab.TaxAccrued += gross - relief;
                lab.TaxSaved += relief;
            }
            else lab.TaxAccrued = MathI.Max(0, lab.TaxAccrued + MathI.MulPermille(taxable, c.Tax.RatePermille));
            lab.TaxResearch += MathI.Max(0, -lab.Ledger.LastWeek.Training);
            lab.TaxExposure += ExposurePerWeek(c, lab.TaxStructure);
            if (Auditing(lab) && state.Tick + 1 >= lab.TaxAuditDue)
            {
                long cost = LapseCost(c, lab);
                Finance.Spend(lab, LedgerLine.Fines, cost);
                lab.TaxStructure = 0;
                lab.TaxBuilding = 0;
                lab.TaxDue = 0;
                lab.TaxExposure = 0;
                Close(c, state, lab, cost, Lapsed);
            }
            if (GameDate.IsYearEnd(c.Calendar, state.Tick)) Pay(c, state, lab);
        }
    }
}
