using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>The litigation docket. See <see cref="LitigationConfig"/>. Only the player's lab is sued.</summary>
    public static class Litigation
    {
        public const int Incident = 0;
        public const int Dismissal = 1;
        public const int Privacy = 2;
        public const int Charter = 3;
        public const int Tax = 4;

        public const int Settled = 0;
        public const int Won = 1;
        public const int Lost = 2;
        public const int Overturned = 3;

        public static bool Enabled(GameConfig c) { return c.Litigation.Enabled > 0; }

        public static LawCase Find(LabState lab, int id)
        {
            for (int i = 0; i < lab.Cases.Count; i++) if (lab.Cases[i].Id == id) return lab.Cases[i];
            return null;
        }

        public static long SettleCost(GameConfig c, LawCase lawCase) { return MathI.MulPermille(lawCase.Claim, c.Litigation.SettlePermille); }
        public static long FirmFee(GameConfig c, LawCase lawCase) { return MathI.MulPermille(lawCase.Claim, c.Litigation.FirmFeePermille); }
        public static long AppealFee(GameConfig c, LawCase lawCase) { return MathI.MulPermille(lawCase.Claim, c.Litigation.AppealFeePermille); }
        public static long Judgment(GameConfig c, LawCase lawCase) { return MathI.MulPermille(lawCase.Claim, c.Litigation.JudgmentPermille); }

        /// <summary>What the counsel takes off the claim's strength.</summary>
        public static int CounselStrength(GameConfig c, LawCase lawCase) { return lawCase.Counsel == 1 ? c.Litigation.FirmStrength : c.Litigation.InHouseStrength; }

        /// <summary>True when the claim would win if it were heard now (after the appeal, if there was one).</summary>
        public static bool WouldWin(GameConfig c, LawCase lawCase)
        {
            int against = CounselStrength(c, lawCase) + (lawCase.Appealed == 1 ? c.Litigation.AppealStrength : 0);
            return lawCase.Strength - against >= c.Litigation.JudgeAt;
        }

        public static int AverageMorale(LabState lab)
        {
            if (lab.Staff.Count == 0) return MathI.Permille / 2;
            long sum = 0;
            for (int i = 0; i < lab.Staff.Count; i++) sum += lab.Staff[i].Morale;
            return (int)(sum / lab.Staff.Count);
        }

        /// <summary>Files a claim if the lab is the player's, the docket has room and the claim is big enough. Returns the claim or null.</summary>
        public static LawCase File(GameConfig c, GameState state, LabState lab, int kind, long claim, int strength, string reference)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || lab.Bankrupt) return null;
            if (lab.Cases.Count >= c.Litigation.MaxOpen) return null;
            claim = MathI.Max(claim, c.Litigation.MinClaim);
            var lawCase = new LawCase();
            lab.CaseSeq++;
            lawCase.Id = lab.CaseSeq;
            lawCase.Kind = kind;
            lawCase.Claim = claim;
            lawCase.Strength = (int)MathI.Clamp(strength, 1, MathI.Permille);
            lawCase.Due = state.Tick + c.Litigation.HearingWeeks;
            lawCase.Ref = reference ?? "";
            lab.Cases.Add(lawCase);
            Events.Add(state, EventKind.CaseFiled, lab.Id, lawCase.Id, claim, lawCase.Ref).Pick = kind;
            return lawCase;
        }

        public static void OnIncident(GameConfig c, GameState state, LabState lab, ModelState model, long cost)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            int strength = MathI.Permille + 100 - SafetyCulture.Level(c, lab) + model.Incidents * 25 - ScalingPolicy.ClaimStrength(c, lab);
            File(c, state, lab, Incident, MathI.MulPermille(cost, c.Litigation.IncidentPermille), strength, model.Name);
        }

        public static void OnLayoff(GameConfig c, GameState state, LabState lab, int sharePermille, long severance, int people)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || people < 1) return;
            int strength = MathI.Permille - AverageMorale(lab) + (sharePermille >= c.Layoffs.DeepPermille ? 100 : 0);
            File(c, state, lab, Dismissal, MathI.MulPermille(severance, c.Litigation.DismissalPermille), strength, "");
        }

        static void Close(GameConfig c, GameState state, LabState lab, LawCase lawCase, long paid, int outcome)
        {
            lab.Cases.Remove(lawCase);
            Events.Add(state, EventKind.CaseClosed, lab.Id, lawCase.Id, paid, lawCase.Ref).Pick = outcome;
        }

        public static void Settle(GameConfig c, GameState state, LabState lab, LawCase lawCase)
        {
            long cost = SettleCost(c, lawCase);
            Finance.Spend(lab, LedgerLine.Fines, cost);
            Close(c, state, lab, lawCase, cost, Settled);
        }

        public static void Retain(GameConfig c, LabState lab, LawCase lawCase)
        {
            Finance.Spend(lab, LedgerLine.Fines, FirmFee(c, lawCase));
            lawCase.Counsel = 1;
        }

        public static void Appeal(GameConfig c, GameState state, LabState lab, LawCase lawCase)
        {
            Finance.Spend(lab, LedgerLine.Fines, AppealFee(c, lawCase));
            lawCase.Appealed = 1;
            lawCase.Due = state.Tick + c.Litigation.AppealWeeks;
        }

        static void Pay(GameConfig c, GameState state, LabState lab, LawCase lawCase)
        {
            long owed = Judgment(c, lawCase);
            Finance.Spend(lab, LedgerLine.Fines, owed);
            lab.Reputation = (int)MathI.Max(0, lab.Reputation - PressSystem.Damp(c, lab, c.Litigation.LossReputation));
            Close(c, state, lab, lawCase, owed, Lost);
        }

        static void Hear(GameConfig c, GameState state, LabState lab, LawCase lawCase)
        {
            if (lawCase.Stage == 0)
            {
                if (WouldWin(c, lawCase))
                {
                    lawCase.Stage = 1;
                    lawCase.Due = state.Tick + c.Litigation.AppealWeeks;
                    Events.Add(state, EventKind.CaseJudged, lab.Id, lawCase.Id, Judgment(c, lawCase), lawCase.Ref).Pick = 0;
                    return;
                }
                PressSystem.AddReputation(lab, c.Litigation.WinReputation);
                Close(c, state, lab, lawCase, 0, Won);
                return;
            }
            if (lawCase.Appealed == 0) { Pay(c, state, lab, lawCase); return; }
            if (WouldWin(c, lawCase))
            {
                Events.Add(state, EventKind.CaseJudged, lab.Id, lawCase.Id, Judgment(c, lawCase), lawCase.Ref).Pick = 1;
                Pay(c, state, lab, lawCase);
                return;
            }
            Close(c, state, lab, lawCase, 0, Overturned);
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Cases.Count == 0 || lab.Id != state.PlayerLabId) return;
            for (int i = 0; i < lab.Cases.Count; i++)
            {
                LawCase lawCase = lab.Cases[i];
                if (state.Tick + 1 < lawCase.Due) continue;
                Hear(c, state, lab, lawCase);
                i = -1;
            }
        }
    }
}
