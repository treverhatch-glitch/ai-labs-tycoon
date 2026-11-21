using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>Government contracts. See <see cref="GovContractsConfig"/>. Only the player's lab is offered any.</summary>
    public static class GovContracts
    {
        public const string RoleId = "engineer";

        public const int Ended = 0;
        public const int Done = 1;
        public const int Leaked = 2;
        public const int Abandoned = -1;

        public static bool Enabled(GameConfig c) { return c.GovContracts.Enabled > 0; }

        public static bool InForce(LabState lab) { return lab.ContractKind.Length > 0; }

        public static bool Offered(LabState lab) { return lab.OfferKind.Length > 0; }

        /// <summary>Engineers kept on the contract in force.</summary>
        public static int People(GameConfig c, LabState lab)
        {
            ContractKindConfig kind = InForce(lab) ? c.GovContracts.Kind(lab.ContractKind) : null;
            return kind == null ? 0 : kind.People;
        }

        /// <summary>Weeks from one instalment to the next.</summary>
        public static int Interval(GameConfig c, ContractKindConfig kind) { return (int)MathI.Max(1, kind.Weeks / c.GovContracts.Installments); }

        public static long InstalmentPay(GameConfig c, ContractKindConfig kind) { return kind.Pay / c.GovContracts.Installments; }

        public static long Penalty(GameConfig c, ContractKindConfig kind) { return MathI.MulPermille(kind.Pay, c.GovContracts.PenaltyPermille); }

        /// <summary>Engineers who could take up a contract now: not on leave, not on the support desk, less those the lab must keep free and those already on one.</summary>
        public static int Spare(GameConfig c, LabState lab)
        {
            return SupportDesk.Available(lab) - SupportDesk.OnDesk(c, lab) - People(c, lab) - c.GovContracts.KeepFree;
        }

        /// <summary>True when the person is on the contract in force: the last engineers in the order the lab lists them who are not on leave.</summary>
        public static bool Away(GameConfig c, LabState lab, StaffMember person)
        {
            int n = People(c, lab);
            if (n <= 0 || person.RoleId != RoleId || person.LeaveDue > 0) return false;
            int seen = 0;
            for (int i = lab.Staff.Count - 1; i >= 0; i--)
            {
                StaffMember other = lab.Staff[i];
                if (other.RoleId != RoleId || other.LeaveDue > 0) continue;
                if (other == person) return seen < n;
                seen++;
            }
            return false;
        }

        /// <summary>The first problem with accepting the contract on offer, or null.</summary>
        public static string AcceptProblem(GameConfig c, LabState lab)
        {
            ContractKindConfig kind = c.GovContracts.Kind(lab.OfferKind);
            if (kind == null) return "There is no contract on offer.";
            if (InForce(lab)) return "The lab already has a contract in force.";
            if (lab.SecurityTier < kind.Clearance) return "The " + kind.Name.ToLowerInvariant() + " contract needs security tier " + kind.Clearance + ".";
            if (Spare(c, lab) < kind.People) return "The contract needs " + kind.People + " engineers who are free for the whole term.";
            return null;
        }

        public static void Accept(GameConfig c, GameState state, LabState lab)
        {
            ContractKindConfig kind = c.GovContracts.Kind(lab.OfferKind);
            lab.ContractKind = kind.Id;
            lab.ContractStart = state.Tick;
            lab.ContractDue = state.Tick + kind.Weeks;
            lab.ContractPaid = 0;
            lab.ContractMissed = 0;
            lab.OfferKind = "";
            lab.OfferDue = 0;
            lab.OfferSeq++;
            if (kind.Morale > 0)
                for (int i = 0; i < lab.Staff.Count; i++)
                    if (lab.Staff[i].RoleId == c.GovContracts.MoraleRole) lab.Staff[i].Morale = (int)MathI.Max(0, lab.Staff[i].Morale - kind.Morale);
            lab.Reputation = (int)MathI.Clamp(lab.Reputation + kind.AcceptReputation, 0, MathI.Permille);
            Events.Add(state, EventKind.ContractAccepted, lab.Id, -1, kind.Pay, kind.Id).Pick = kind.Weeks;
        }

        public static void Decline(GameConfig c, GameState state, LabState lab, bool turnedDown)
        {
            string id = lab.OfferKind;
            lab.OfferKind = "";
            lab.OfferDue = 0;
            lab.OfferSeq++;
            lab.ContractNext = state.Tick + c.GovContracts.OfferEveryWeeks;
            Events.Add(state, EventKind.ContractDeclined, lab.Id, -1, 0, id).Pick = turnedDown ? 1 : 0;
        }

        static void End(GameConfig c, GameState state, LabState lab, ContractKindConfig kind, int outcome)
        {
            long penalty = 0;
            if (outcome != Done)
            {
                penalty = Penalty(c, kind);
                Finance.Spend(lab, LedgerLine.Fines, penalty);
                lab.Reputation = (int)MathI.Max(0, lab.Reputation - PressSystem.Damp(c, lab, outcome == Abandoned ? c.GovContracts.AbandonReputation : c.GovContracts.AbandonReputation / 2));
            }
            else PressSystem.AddReputation(lab, kind.DoneReputation);
            lab.ContractKind = "";
            lab.ContractStart = 0;
            lab.ContractDue = 0;
            lab.ContractPaid = 0;
            lab.ContractMissed = 0;
            lab.ContractNext = state.Tick + c.GovContracts.OfferEveryWeeks;
            Events.Add(state, EventKind.ContractEnded, lab.Id, -1, penalty, kind.Id).Pick = outcome;
        }

        public static void Abandon(GameConfig c, GameState state, LabState lab)
        {
            End(c, state, lab, c.GovContracts.Kind(lab.ContractKind), Abandoned);
        }

        /// <summary>A leaked breach ends any contract that needed clearance.</summary>
        public static void OnLeak(GameConfig c, GameState state, LabState lab)
        {
            if (!InForce(lab) || lab.Id != state.PlayerLabId) return;
            ContractKindConfig kind = c.GovContracts.Kind(lab.ContractKind);
            if (kind != null && kind.Clearance > 0) End(c, state, lab, kind, Leaked);
        }

        static void Instalment(GameConfig c, GameState state, LabState lab, ContractKindConfig kind)
        {
            bool met = SupportDesk.Available(lab) - SupportDesk.OnDesk(c, lab) >= kind.People;
            if (met)
            {
                long pay = InstalmentPay(c, kind);
                Finance.Earn(lab, LedgerLine.Grants, pay);
                lab.ContractPaid++;
                Events.Add(state, EventKind.ContractInstalment, lab.Id, -1, pay, kind.Id).Pick = 1;
            }
            else
            {
                lab.ContractMissed++;
                Events.Add(state, EventKind.ContractInstalment, lab.Id, -1, 0, kind.Id).Pick = 0;
            }
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            if (InForce(lab))
            {
                ContractKindConfig kind = c.GovContracts.Kind(lab.ContractKind);
                int step = Interval(c, kind);
                int week = state.Tick + 1 - lab.ContractStart;
                int owed = (int)MathI.Min(c.GovContracts.Installments, week / step);
                while (lab.ContractPaid + lab.ContractMissed < owed)
                {
                    Instalment(c, state, lab, kind);
                    if (lab.ContractMissed >= c.GovContracts.MissLimit) { End(c, state, lab, kind, Ended); return; }
                }
                if (state.Tick + 1 >= lab.ContractDue)
                {
                    while (lab.ContractPaid + lab.ContractMissed < c.GovContracts.Installments)
                    {
                        Instalment(c, state, lab, kind);
                        if (lab.ContractMissed >= c.GovContracts.MissLimit) { End(c, state, lab, kind, Ended); return; }
                    }
                    End(c, state, lab, kind, Done);
                }
                return;
            }
            if (Offered(lab))
            {
                if (state.Tick + 1 >= lab.OfferDue) Decline(c, state, lab, false);
                return;
            }
            if (Security.OnSale(lab) == 0) return;
            if (lab.ContractNext == 0) lab.ContractNext = state.Tick + 1 + c.GovContracts.OfferEveryWeeks;
            if (state.Tick + 1 < lab.ContractNext) return;
            ContractKindConfig next = c.GovContracts.Kinds[lab.OfferSeq % c.GovContracts.Kinds.Count];
            lab.OfferKind = next.Id;
            lab.OfferDue = state.Tick + 1 + c.GovContracts.OfferWeeks;
            Events.Add(state, EventKind.ContractOffered, lab.Id, -1, next.Pay, next.Id).Pick = next.Weeks;
        }
    }
}
