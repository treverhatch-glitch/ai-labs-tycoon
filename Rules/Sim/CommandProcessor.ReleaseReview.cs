using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SubmitReleaseReview(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (!ReleaseReviewSystem.GateOpen(c, state, lab))
                return Fail(CommandError.InvalidArgument, "Release review is not in force yet.");
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Status != ModelStatus.Trained)
                return Fail(CommandError.WrongModelStatus, model.Name + " must finish training before review.");
            if (ReleaseReviewSystem.CaseForModel(lab, model.Id) != null)
                return Fail(CommandError.InvalidArgument, "That model already has an open review.");
            CommandResult sectors = ReleaseReviewSystem.ValidateSectors(c, state, lab, model, cmd.Ids);
            if (!sectors.Ok) return sectors;
            if (!ReleaseReviewSystem.NeedsReviewBeforeRelease(c, state, lab, model, cmd.Ids, cmd.Permille))
                return Fail(CommandError.InvalidArgument, "That package does not need review; release it directly.");

            int claimed = ReleaseReviewSystem.EffectiveClaimedPermille(c, model, cmd.Permille);
            var review = new ReleaseReviewCase
            {
                ModelId = model.Id,
                Phase = ReleaseReviewPhase.Submitted,
                ClaimedAlignmentPermille = claimed,
                SubmittedTick = state.Tick,
            };
            for (int i = 0; i < cmd.Ids.Count; i++) review.SectorIds.Add(cmd.Ids[i]);
            lab.ReleaseReviews.Add(review);
            Events.Add(state, EventKind.ReleaseReviewSubmitted, lab.Id, model.Id, cmd.Ids.Count, model.Name);
            return CommandResult.Done(model.Id);
        }

        static CommandResult ReviseReleaseReview(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            ReleaseReviewCase review = ReleaseReviewSystem.CaseForModel(lab, model.Id);
            if (review == null || review.Phase != ReleaseReviewPhase.NeedsRevision)
                return Fail(CommandError.ReleaseReviewNotReady, "There is no revision waiting for that model.");
            if (cmd.Permille < 1) return Fail(CommandError.InvalidArgument, "Name the alignment you will claim.");
            review.ClaimedAlignmentPermille = (int)MathI.Min(MathI.Permille, cmd.Permille);
            review.Phase = ReleaseReviewPhase.Submitted;
            review.SubmittedTick = state.Tick;
            return CommandResult.Done(model.Id);
        }

        static CommandResult AppealReleaseReview(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            ReleaseReviewCase review = ReleaseReviewSystem.CaseForModel(lab, model.Id);
            if (review == null || review.Phase != ReleaseReviewPhase.NeedsRevision)
                return Fail(CommandError.ReleaseReviewNotReady, "Only a sent-back package can be appealed.");
            ReleaseReviewConfig cfg = c.ReleaseReview;
            if (cfg.AppealCashCost > 0 && lab.Cash < cfg.AppealCashCost) return NeedCash(lab, cfg.AppealCashCost);
            ReleaseReviewSystem.TryAppeal(c, state, lab, review, model);
            return CommandResult.Done(model.Id);
        }

        static CommandResult WithdrawReleaseReview(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            ReleaseReviewCase review = ReleaseReviewSystem.CaseForModel(lab, model.Id);
            if (review == null) return Fail(CommandError.InvalidArgument, "There is no open review for that model.");
            if (review.Phase == ReleaseReviewPhase.Approved)
                return Fail(CommandError.InvalidArgument, "An approved package cannot be withdrawn; release or wait.");
            review.Phase = ReleaseReviewPhase.Withdrawn;
            return CommandResult.Done(model.Id);
        }

        static CommandResult ExpediteReleaseReview(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            ReleaseReviewCase review = ReleaseReviewSystem.CaseForModel(lab, model.Id);
            if (review == null || review.Phase != ReleaseReviewPhase.Submitted)
                return Fail(CommandError.ReleaseReviewNotReady, "Only a queued submission can be expedited.");
            if (review.Expedited) return Fail(CommandError.InvalidArgument, "That package is already expedited.");
            long cost = c.ReleaseReview.ExpediteCashCost;
            if (cost > 0 && lab.Cash < cost) return NeedCash(lab, cost);
            if (cost > 0) Finance.Spend(lab, LedgerLine.Fines, cost);
            review.Expedited = true;
            return CommandResult.Done(model.Id);
        }
    }
}