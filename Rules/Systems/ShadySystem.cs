using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class ShadySystem
    {
        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Bankrupt) return;
            bool resolved = false;
            for (int i = lab.ShadyOps.Count - 1; i >= 0; i--)
            {
                ShadyOp op = lab.ShadyOps[i];
                if (op.ResolveTick > state.Tick) continue;
                resolved = true;
                ResolveOne(c, state, lab, op);
                lab.ShadyOps.RemoveAt(i);
            }
            for (int i = 0; i < lab.ShadyOps.Count; i++)
            {
                ShadyOp op = lab.ShadyOps[i];
                if (op.ResolveTick <= state.Tick) continue;
                ShadyCardConfig card = c.Shady.Card(op.CardId);
                if (card == null || card.Special != ShadySpecial.GreyCompute || op.AuxId < 0) continue;
                Finance.Earn(lab, LedgerLine.GreyRevenue, GreyPayout(c, state, lab, op.AuxId));
            }
            if (!resolved && lab.Heat > 0)
            {
                int drop = ShadyQueries.PolicyHeatDecay(c, lab);
                lab.Heat = (int)MathI.Max(0, lab.Heat - drop);
            }
            SideJobs(c, state, lab);
            Temptation(c, state, lab);
        }

        public static void RunMonth(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Bankrupt) return;
            if (lab.Id != state.PlayerLabId && lab.Controller == LabController.Plan)
            {
                RivalConfig rival = c.Rival(lab.RivalId);
                if (rival == null || rival.Plan.ShadyAppetite <= 0) return;
            }
            int tick = state.Tick;
            if (lab.CaseEndsTick >= 0 && tick >= lab.CaseEndsTick)
            {
                CloseCase(c, state, lab);
                return;
            }
            if (ShadyQueries.CaseOpen(lab, tick)) return;
            if (lab.Heat < c.Shady.InvestigationThreshold) return;
            int permille = (int)(MathI.Max(0, lab.Heat - c.Shady.InvestigationThreshold) / c.Shady.InvestigationDivisor);
            if (state.Rng.Shady.Chance(permille))
            {
                lab.CaseEndsTick = tick + c.Shady.CaseWeeks;
                Events.Add(state, EventKind.ShadyInvestigation, lab.Id, -1, lab.Heat, "");
            }
        }

        public static void OnAuditOpen(GameConfig c, GameState state, LabState lab)
        {
            if (ShadyQueries.CaseOpen(lab, state.Tick)) return;
            TryOpenCase(c, state, lab, "audit");
        }

        static void TryOpenCase(GameConfig c, GameState state, LabState lab, string tag)
        {
            if (lab.Heat < c.Shady.InvestigationThreshold) return;
            int permille = (int)(MathI.Max(0, lab.Heat - c.Shady.InvestigationThreshold) / c.Shady.InvestigationDivisor);
            if (!state.Rng.Shady.Chance(permille)) return;
            lab.CaseEndsTick = state.Tick + c.Shady.CaseWeeks;
            Events.Add(state, EventKind.ShadyInvestigation, lab.Id, -1, lab.Heat, tag);
        }

        static void CloseCase(GameConfig c, GameState state, LabState lab)
        {
            ShadyConfig cfg = c.Shady;
            int staff = WorldQueries.PolicyStaff(c, lab);
            int chance = (int)MathI.Clamp(lab.Heat / cfg.ConvictionDivisor - staff * cfg.ConvictionPolicyPerStaff, 0, 900);
            lab.CaseEndsTick = -1;
            if (state.Rng.Shady.Chance(chance))
            {
                Events.Add(state, EventKind.ShadyConvicted, lab.Id, -1, lab.Heat, "");
                EndingSystem.RecordHandcuffs(c, state, lab);
            }
            else
            {
                lab.Heat = (int)MathI.Max(0, lab.Heat - cfg.AcquittalHeatDrop);
                Events.Add(state, EventKind.ShadyAcquitted, lab.Id, -1, lab.Heat, "");
            }
        }

        static void ResolveOne(GameConfig c, GameState state, LabState lab, ShadyOp op)
        {
            ShadyCardConfig card = c.Shady.Card(op.CardId);
            ModelState model = lab.Model(op.ModelId);
            if (card == null || model == null) return;
            LabState target = op.TargetLabId >= 0 ? state.Lab(op.TargetLabId) : null;
            ShadyOddsBreakdown odds = ShadyQueries.SuccessOdds(c, state, lab, card, model, TargetForDefense(c, state, op, card));
            bool success = state.Rng.Shady.Chance(ShadyQueries.ClampedSuccess(odds));
            bool traced = state.Rng.Shady.Chance(ShadyQueries.TracePermille(c, lab, card));

            if (success) OnSuccess(c, state, lab, op, card, model, target);
            else OnFail(c, state, lab, op, card, model, target);

            int corr = success ? card.Corruption : card.Corruption / 2;
            model.Corruption = (int)MathI.Min(MathI.Permille, model.Corruption + corr);
            if (traced) OnTraced(c, state, lab, op, card, model, target, success);
            Events.Add(state, EventKind.ShadyOpResolved, lab.Id, op.Id, success ? 1 : 0, card.Id);
        }

        static LabState TargetForDefense(GameConfig c, GameState state, ShadyOp op, ShadyCardConfig card)
        {
            if (card.Special == ShadySpecial.SabotageRun || card.Special == ShadySpecial.CopyModel) return state.Lab(op.TargetLabId);
            return null;
        }

        static void OnSuccess(GameConfig c, GameState state, LabState lab, ShadyOp op, ShadyCardConfig card, ModelState model, LabState target)
        {
            lab.Heat = (int)MathI.Min(MathI.Permille, lab.Heat + card.HeatOk);
            long payout = card.BasePayout;
            if (card.MinCap > 0 && card.BasePayout > 0) payout = MathI.MulPermille(card.BasePayout, model.Capability * MathI.Permille / 100);
            if (card.PayoutTo == ShadyPayout.Stash) lab.Stash += payout;
            else if (card.PayoutTo == ShadyPayout.Cash && card.Special != ShadySpecial.GreyCompute)
                Finance.Earn(lab, LedgerLine.GreyRevenue, payout);
            if (card.Id == "crypto-heist")
            {
                for (int i = 0; i < lab.Blocs.Count; i++) lab.Blocs[i].Attention = (int)MathI.Min(MathI.Permille, lab.Blocs[i].Attention + 80);
            }

            switch (card.Special)
            {
                case ShadySpecial.CopyModel: CopyWeights(c, state, lab, target, op); break;
                case ShadySpecial.SabotageRun: Sabotage(c, state, lab, target); break;
                case ShadySpecial.HackGovernment: HackGov(c, lab, op.TargetRef, false); break;
                case ShadySpecial.SmuggleChips: Smuggle(c, state, lab, op); break;
                case ShadySpecial.GreyCompute:
                    break;
            }
        }

        static void OnFail(GameConfig c, GameState state, LabState lab, ShadyOp op, ShadyCardConfig card, ModelState model, LabState target)
        {
            lab.Heat = (int)MathI.Min(MathI.Permille, lab.Heat + card.HeatFail);
            if (card.Special == ShadySpecial.HackGovernment) HackGov(c, lab, op.TargetRef, true);
            if (card.Id == "crypto-heist")
            {
                for (int i = 0; i < lab.Blocs.Count; i++) lab.Blocs[i].Attention = (int)MathI.Min(MathI.Permille, lab.Blocs[i].Attention + 80);
            }
        }

        static void OnTraced(GameConfig c, GameState state, LabState lab, ShadyOp op, ShadyCardConfig card, ModelState model, LabState target, bool success)
        {
            Events.Add(state, EventKind.ShadyTraced, lab.Id, op.ModelId, 0, card.Id);
            BlocStanding home = lab.Bloc(lab.HomeBlocId);
            if (home != null) home.Attention = (int)MathI.Min(MathI.Permille, home.Attention + 100);
            if (card.Special == ShadySpecial.SabotageRun)
            {
                VendorSystem.Change(c, state, lab.Id, -200, true);
                for (int i = 0; i < state.Labs.Count; i++)
                    if (state.Labs[i].Controller == LabController.Plan) RivalSystem.Feel(c, state.Labs[i], c.Rivalry.Feelings.SabotageAllPermille);
            }
            else if (card.Special == ShadySpecial.CopyModel && target != null)
            {
                ModelState src = target.Model(op.AuxId);
                ModelLineConfig line = src != null ? c.Line(src.LineId) : null;
                if (line != null)
                {
                    long fine = 2L * EraSystem.RunPrepCost(c, line, state.Tick);
                    Finance.Spend(lab, LedgerLine.Fines, fine);
                }
                RivalSystem.Feel(c, target, c.Rivalry.Feelings.TracedShadyPermille);
            }
        }

        static long GreyPayout(GameConfig c, GameState state, LabState lab, int holdingId)
        {
            Holding h = lab.Holding(holdingId);
            if (h == null) return 0;
            long kw = WorldQueries.InstalledKw(c, state, lab, holdingId) / 2;
            ShadyCardConfig card = c.Shady.Card("grey-compute");
            return card == null ? 0 : card.BasePayout * kw;
        }

        static void CopyWeights(GameConfig c, GameState state, LabState lab, LabState target, ShadyOp op)
        {
            if (target == null) return;
            ModelState src = target.Model(op.AuxId);
            if (src == null) return;
            ShadyConfig cfg = c.Shady;
            var m = new ModelState();
            m.Id = state.TakeId();
            m.LineId = src.LineId;
            m.Version = LabQueries.NextVersion(lab, src.LineId);
            m.Name = lab.FamilyName + " " + c.Line(src.LineId).Name + " " + m.Version;
            m.Status = ModelStatus.Trained;
            m.CapBase = (int)MathI.MulPermille(src.CapBase, cfg.CopyCapabilityPermille);
            m.CapTech = (int)MathI.MulPermille(src.CapTech, cfg.CopyCapabilityPermille);
            m.CapCarryover = (int)MathI.MulPermille(src.CapCarryover, cfg.CopyCapabilityPermille);
            m.CapLuck = 0;
            m.Capability = m.CapBase + m.CapTech + m.CapCarryover;
            m.Alignment = (int)MathI.MulPermille(src.Alignment, cfg.CopyAlignmentPermille);
            m.Corruption = cfg.CopyCorruption;
            m.StartedTick = -1;
            m.TrainedTick = state.Tick;
            m.ReleasedTick = -1;
            m.LastIncidentTick = -1;
            lab.Models.Add(m);
        }

        static ModelState BestReleasedOfLine(GameConfig c, LabState lab, string lineId)
        {
            ModelState best = null;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.LineId != lineId || m.Status != ModelStatus.Released) continue;
                if (best == null || m.Capability > best.Capability) best = m;
            }
            return best;
        }

        static void Sabotage(GameConfig c, GameState state, LabState lab, LabState target)
        {
            if (target == null) return;
            ModelState run = null;
            for (int i = 0; i < target.Models.Count; i++)
            {
                ModelState m = target.Models[i];
                if (m.Status != ModelStatus.Training) continue;
                if (run == null || m.StartedTick < run.StartedTick) run = m;
            }
            if (run == null) return;
            run.ProgressCompute = MathI.MulPermille(run.ProgressCompute, c.Shady.SabotageProgressPermille);
            Events.Add(state, EventKind.ShadyOpResolved, target.Id, run.Id, run.ProgressCompute, "sabotage");
        }

        static void HackGov(GameConfig c, LabState lab, string blocId, bool fail)
        {
            if (string.IsNullOrEmpty(blocId)) return;
            BlocStanding s = lab.Bloc(blocId);
            if (s == null) return;
            int delta = fail ? 400 : -300;
            s.Attention = (int)MathI.Clamp(s.Attention + delta, 0, MathI.Permille);
        }

        static void Smuggle(GameConfig c, GameState state, LabState lab, ShadyOp op)
        {
            HardwareBatch batch = lab.Batch(op.AuxId);
            if (batch == null || batch.ChipDesignId < 0) return;
            if (!int.TryParse(op.TargetRef, out int dest) || lab.Holding(dest) == null) return;
            batch.HoldingId = dest;
        }

        static void SideJobs(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return;
            ShadyConfig cfg = c.Shady;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.Status != ModelStatus.Released || m.Corruption < cfg.SideJobFrom) continue;
                if (!state.Rng.Shady.Chance(cfg.SideJobPermille)) continue;
                ShadyCardConfig card = OpenStashCard(c, state);
                if (card == null) continue;
                long payout = MathI.MulPermille(card.BasePayout, m.Capability * MathI.Permille / 100);
                lab.Stash += payout;
                lab.Heat = (int)MathI.Min(MathI.Permille, lab.Heat + card.HeatOk);
                m.Corruption = (int)MathI.Min(MathI.Permille, m.Corruption + 50);
                Events.Add(state, EventKind.ShadySideJob, lab.Id, m.Id, payout, m.Name);
            }
        }

        static ShadyCardConfig OpenStashCard(GameConfig c, GameState state)
        {
            ShadyCardConfig best = null;
            for (int i = 0; i < c.Shady.Cards.Count; i++)
            {
                ShadyCardConfig card = c.Shady.Cards[i];
                if (card.PayoutTo != ShadyPayout.Stash) continue;
                if (!ShadyQueries.CardOpen(c, card, state.Tick)) continue;
                if (best == null || card.BasePayout > best.BasePayout) best = card;
            }
            return best;
        }

        static void Temptation(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return;
            long costs = LabQueries.WeeklySalaries(c, lab) + LabQueries.WeeklyRoomUpkeep(c, lab) + LabQueries.WeeklyGpuRent(c, state, lab);
            if (costs <= 0 || lab.Cash >= costs * c.Shady.TemptationRunwayWeeks) return;
            ShadyCardConfig card = OpenStashCard(c, state);
            if (card == null) return;
            Events.Add(state, EventKind.ShadyTemptation, lab.Id, -1, 0, card.Id);
        }
    }
}