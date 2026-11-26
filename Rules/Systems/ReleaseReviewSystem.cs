using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
  /// <summary>
  /// Player-only release board: submit packages monthly, staff resolves sandbagging and enterprise sector risk,
  /// then ReleaseModel may only match an approved package.
  /// </summary>
  public static class ReleaseReviewSystem
  {
    public static bool GateOpen(GameConfig c, GameState state, LabState lab)
    {
      if (lab == null || lab.Id != state.PlayerLabId) return false;
      return GameDate.Year(c.Calendar, state.Tick) >= c.ReleaseReview.OpensYear;
    }

    public static bool SectorStrict(GameConfig c, string sectorId)
    {
      List<string> ids = c.ReleaseReview.StrictSectorIds;
      for (int i = 0; i < ids.Count; i++) if (ids[i] == sectorId) return true;
      return false;
    }

    public static bool AnyStrictSector(GameConfig c, IList<string> sectorIds)
    {
      for (int i = 0; i < sectorIds.Count; i++) if (SectorStrict(c, sectorIds[i])) return true;
      return false;
    }

    public static int EffectiveClaimedPermille(GameConfig c, ModelState model, int cmdPermille)
    {
      if (cmdPermille > 0) return (int)MathI.Min(MathI.Permille, cmdPermille);
      return (int)MathI.Max(model.Alignment, model.DisplayedAlignment);
    }

    public static bool NeedsReviewBeforeRelease(GameConfig c, GameState state, LabState lab, ModelState model, IList<string> sectorIds, int cmdPermille)
    {
      if (!GateOpen(c, state, lab) || model == null || sectorIds == null || sectorIds.Count == 0) return false;
      int claimed = EffectiveClaimedPermille(c, model, cmdPermille);
      if (AnyStrictSector(c, sectorIds)) return true;
      return claimed > model.Alignment;
    }

    public static ReleaseReviewCase CaseForModel(LabState lab, int modelId)
    {
      for (int i = 0; i < lab.ReleaseReviews.Count; i++)
      {
        ReleaseReviewCase r = lab.ReleaseReviews[i];
        if (r.ModelId == modelId && r.Phase != ReleaseReviewPhase.Withdrawn && r.Phase != ReleaseReviewPhase.Rejected)
          return r;
      }
      return null;
    }

    public static bool HasOpenCase(LabState lab, int modelId)
    {
      ReleaseReviewCase r = CaseForModel(lab, modelId);
      return r != null && r.Phase != ReleaseReviewPhase.Approved;
    }

    public static ReleaseReviewCase FindApproved(LabState lab, int modelId, IList<string> sectorIds, int claimedPermille)
    {
      for (int i = 0; i < lab.ReleaseReviews.Count; i++)
      {
        ReleaseReviewCase r = lab.ReleaseReviews[i];
        if (r.ModelId != modelId || r.Phase != ReleaseReviewPhase.Approved) continue;
        if (r.ClaimedAlignmentPermille != claimedPermille) continue;
        if (!SameSectors(r.SectorIds, sectorIds)) continue;
        return r;
      }
      return null;
    }

    public static void ConsumeApproval(LabState lab, ReleaseReviewCase review)
    {
      if (review != null) lab.ReleaseReviews.Remove(review);
    }

    public static int MonthlySlots(GameConfig c, LabState lab)
    {
      ReleaseReviewConfig r = c.ReleaseReview;
      int staff = LabQueries.StaffByFunction(c, lab, RoleFunction.Safety);
      int slots = r.BaseSlotsPerMonth + staff * r.SlotsPerSafetyStaff;
      return (int)MathI.Min(r.MaxSlotsPerMonth, MathI.Max(0, slots));
    }

    public static int PendingCount(LabState lab)
    {
      int n = 0;
      for (int i = 0; i < lab.ReleaseReviews.Count; i++)
      {
        ReleaseReviewPhase p = lab.ReleaseReviews[i].Phase;
        if (p == ReleaseReviewPhase.Submitted || p == ReleaseReviewPhase.NeedsRevision) n++;
      }
      return n;
    }

    public static void RunMonth(GameConfig c, GameState state, LabState lab)
    {
      if (!GateOpen(c, state, lab)) return;
      ReleaseReviewConfig cfg = c.ReleaseReview;

      int pending = PendingCount(lab);
      if (pending > cfg.MaxBacklogBeforePenalty && cfg.BacklogReputationPenalty > 0)
      {
        int over = pending - cfg.MaxBacklogBeforePenalty;
        lab.Reputation = (int)MathI.Max(0, lab.Reputation - over * cfg.BacklogReputationPenalty);
      }

      var queue = new List<ReleaseReviewCase>();
      for (int i = 0; i < lab.ReleaseReviews.Count; i++)
      {
        ReleaseReviewCase r = lab.ReleaseReviews[i];
        if (r.Phase == ReleaseReviewPhase.Submitted || r.Phase == ReleaseReviewPhase.NeedsRevision)
          queue.Add(r);
      }
      queue.Sort((a, b) =>
      {
        if (a.Expedited != b.Expedited) return b.Expedited.CompareTo(a.Expedited);
        return a.SubmittedTick.CompareTo(b.SubmittedTick);
      });

      int slots = MonthlySlots(c, lab);
      for (int i = 0; i < queue.Count && i < slots; i++) ResolveCase(c, state, lab, queue[i]);
    }

    static void ResolveCase(GameConfig c, GameState state, LabState lab, ReleaseReviewCase review)
    {
      ModelState model = lab.Model(review.ModelId);
      if (model == null)
      {
        review.Phase = ReleaseReviewPhase.Rejected;
        return;
      }

      ReleaseReviewConfig cfg = c.ReleaseReview;
      int gap = review.ClaimedAlignmentPermille - model.Alignment;
      if (gap >= cfg.SandbagSendBackGapPermille)
      {
        review.RevisionRounds++;
        if (review.RevisionRounds > cfg.MaxRevisionRounds)
          FinishReject(c, state, lab, review, model);
        else
        {
          review.Phase = ReleaseReviewPhase.NeedsRevision;
          Events.Add(state, EventKind.ReleaseReviewRevision, lab.Id, model.Id, gap, model.Name);
        }
        return;
      }

      if (AnyStrictSector(c, review.SectorIds) && model.Alignment < cfg.MinAlignmentStrictSector)
      {
        FinishReject(c, state, lab, review, model);
        return;
      }

      review.Phase = ReleaseReviewPhase.Approved;
      Events.Add(state, EventKind.ReleaseReviewApproved, lab.Id, model.Id, review.SectorIds.Count, model.Name);
    }

    static void FinishReject(GameConfig c, GameState state, LabState lab, ReleaseReviewCase review, ModelState model)
    {
      review.Phase = ReleaseReviewPhase.Rejected;
      int loss = c.ReleaseReview.RejectedReputationPenalty;
      if (loss > 0) lab.Reputation = (int)MathI.Max(0, lab.Reputation - loss);
      if (AnyStrictSector(c, review.SectorIds)) lab.IncidentsThisMonth++;
      Events.Add(state, EventKind.ReleaseReviewRejected, lab.Id, model.Id, loss, model.Name);
    }

    public static bool TryAppeal(GameConfig c, GameState state, LabState lab, ReleaseReviewCase review, ModelState model)
    {
      ReleaseReviewConfig cfg = c.ReleaseReview;
      if (cfg.AppealCashCost > 0 && lab.Cash < cfg.AppealCashCost) return false;
      if (cfg.AppealCashCost > 0) Finance.Spend(lab, LedgerLine.Fines, cfg.AppealCashCost);
      if (state.Rng.Safety.Chance(cfg.AppealSuccessPermille))
      {
        review.Phase = ReleaseReviewPhase.Approved;
        Events.Add(state, EventKind.ReleaseReviewApproved, lab.Id, model.Id, review.SectorIds.Count, model.Name);
        return true;
      }
      review.Phase = ReleaseReviewPhase.Rejected;
      int loss = cfg.AppealFailReputationPenalty;
      if (loss > 0) lab.Reputation = (int)MathI.Max(0, lab.Reputation - loss);
      Events.Add(state, EventKind.ReleaseReviewRejected, lab.Id, model.Id, loss, model.Name);
      return true;
    }

    public static bool SameSectors(IList<string> a, IList<string> b)
    {
      if (a.Count != b.Count) return false;
      for (int i = 0; i < a.Count; i++)
      {
        if (b.IndexOf(a[i]) < 0) return false;
      }
      return true;
    }

    public static CommandResult ValidateSectors(GameConfig c, GameState state, LabState lab, ModelState model, IList<string> sectorIds)
    {
      if (sectorIds == null || sectorIds.Count == 0) return CommandResult.Fail(CommandError.InvalidArgument, "Name at least one sector.");
      for (int i = 0; i < sectorIds.Count; i++)
      {
        SectorConfig sector = c.Sector(sectorIds[i]);
        if (sector == null) return CommandResult.Fail(CommandError.UnknownId, "There is no sector \"" + sectorIds[i] + "\".");
        if (sectorIds.IndexOf(sectorIds[i]) != i) return CommandResult.Fail(CommandError.InvalidArgument, sector.Name + " is named twice.");
        if (!LabQueries.SectorOpen(c, sector, state.Tick))
          return CommandResult.Fail(CommandError.SectorClosed, sector.Name + " opens in " + sector.OpensYear + ".");
        if (model.Capability < sector.MinCapability)
          return CommandResult.Fail(CommandError.BelowSectorBar, sector.Name + " needs capability " + sector.MinCapability + ".");
      }
      return CommandResult.Done(-1);
    }
  }
}