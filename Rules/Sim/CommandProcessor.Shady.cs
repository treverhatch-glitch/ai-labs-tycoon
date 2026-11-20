using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult StartShadyOp(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId && (lab.Controller != LabController.Plan || c.Rival(lab.RivalId) == null || c.Rival(lab.RivalId).Plan.ShadyAppetite <= 0))
                return Fail(CommandError.InvalidArgument, "Only the player's lab may run shady jobs.");
            if (ShadyQueries.IsLocked(lab, state.Tick))
                return Fail(CommandError.ShadyLocked, lab.Name + " is locked out of shady work.");
            if (ShadyQueries.CaseOpen(lab, state.Tick))
                return Fail(CommandError.CaseOpen, "An investigation is open; no shady work until it closes.");

            ShadyCardConfig card = c.Shady.Card(cmd.Id);
            if (card == null) return Fail(CommandError.UnknownId, "There is no shady card \"" + cmd.Id + "\".");
            if (!ShadyQueries.CardOpen(c, card, state.Tick))
                return Fail(CommandError.EraLocked, card.Name + " is not around yet.");

            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.NoModelForJob, "There is no model " + cmd.TargetId + ".");
            if (model.Status == ModelStatus.Training)
                return Fail(CommandError.NoModelForJob, model.Name + " is still training.");
            if (model.Status != ModelStatus.Trained && model.Status != ModelStatus.Released)
                return Fail(CommandError.NoModelForJob, model.Name + " cannot run a shady job.");
            if (model.Capability < card.MinCap)
                return Fail(CommandError.NoModelForJob, model.Name + " needs at least " + card.MinCap + " capability.");
            if (ShadyQueries.ModelOnJob(lab, model.Id))
                return Fail(CommandError.NoModelForJob, model.Name + " is already on a shady job.");

            if (!TargetOk(c, state, lab, card, cmd, out string message, out int targetModelId))
                return Fail(CommandError.TargetNotEligible, message);

            var op = new ShadyOp();
            op.Id = state.TakeId();
            op.CardId = card.Id;
            op.ModelId = model.Id;
            op.TargetLabId = cmd.X;
            op.AuxId = card.Special == ShadySpecial.CopyModel ? targetModelId : cmd.Y;
            op.TargetRef = cmd.Ids != null && cmd.Ids.Count > 0 ? cmd.Ids[0] : "";
            op.StartTick = state.Tick;
            op.ResolveTick = state.Tick + card.Weeks;
            lab.ShadyOps.Add(op);
            Events.Add(state, EventKind.ShadyOpStarted, lab.Id, op.Id, card.Weeks, card.Id);
            return CommandResult.Done(op.Id);
        }

        static bool TargetOk(GameConfig c, GameState state, LabState lab, ShadyCardConfig card, Command cmd, out string message, out int targetModelId)
        {
            message = "";
            targetModelId = -1;
            string line = cmd.Ids != null && cmd.Ids.Count > 0 ? cmd.Ids[0] : "";
            switch (card.Special)
            {
                case ShadySpecial.CopyModel:
                    LabState rival = state.Lab(cmd.X);
                    if (rival == null || rival.Id == lab.Id) { message = "Pick a rival lab with a released model to copy."; return false; }
                    ModelState theirs = BestReleasedAbove(c, lab, rival, line);
                    if (theirs == null) { message = rival.Name + " has no released model above yours on that line."; return false; }
                    targetModelId = theirs.Id;
                    return true;
                case ShadySpecial.SabotageRun:
                    LabState t = state.Lab(cmd.X);
                    if (t == null || t.Id == lab.Id) { message = "Pick a rival with a training run in progress."; return false; }
                    if (LabQueries.ActiveRuns(t) < 1) { message = t.Name + " has no run in progress."; return false; }
                    return true;
                case ShadySpecial.HackGovernment:
                    if (cmd.Ids == null || cmd.Ids.Count == 0 || c.World.Bloc(cmd.Ids[0]) == null)
                    { message = "Name a bloc to lean on."; return false; }
                    return true;
                case ShadySpecial.GreyCompute:
                    if (lab.Holding(cmd.Y) == null) { message = "Name your own holding."; return false; }
                    return true;
                case ShadySpecial.SmuggleChips:
                    if (lab.Batch(cmd.Y) == null) { message = "Name a hardware batch to move."; return false; }
                    if (cmd.Ids == null || cmd.Ids.Count == 0 || lab.Holding(int.Parse(cmd.Ids[0])) == null)
                    { message = "Name a destination holding."; return false; }
                    return true;
                default:
                    return true;
            }
        }

        static ModelState BestReleasedAbove(GameConfig c, LabState lab, LabState rival, string lineHint)
        {
            ModelState bestTheirs = null;
            for (int i = 0; i < rival.Models.Count; i++)
            {
                ModelState m = rival.Models[i];
                if (m.Status != ModelStatus.Released) continue;
                if (!string.IsNullOrEmpty(lineHint) && m.LineId != lineHint) continue;
                if (bestTheirs == null || m.Capability > bestTheirs.Capability) bestTheirs = m;
            }
            if (bestTheirs == null) return null;
            int mine = 0;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.LineId != bestTheirs.LineId || m.Status != ModelStatus.Released) continue;
                if (m.Capability > mine) mine = m.Capability;
            }
            return bestTheirs.Capability > mine ? bestTheirs : null;
        }

        static CommandResult Launder(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab can launder.");
            if (ShadyQueries.CaseOpen(lab, state.Tick)) return Fail(CommandError.CaseOpen, "An investigation is open.");
            if (cmd.Count < 1) return Fail(CommandError.InvalidArgument, "Launder at least $1.");
            if (cmd.Count > lab.Stash) return Fail(CommandError.NotEnoughStash, "The stash has $" + lab.Stash + ".");
            long kept = MathI.MulPermille(cmd.Count, c.Shady.LaunderKeepPermille);
            lab.Stash -= cmd.Count;
            Finance.Earn(lab, LedgerLine.Laundered, kept);
            lab.Heat = (int)MathI.Min(MathI.Permille, lab.Heat + cmd.Count / c.Shady.LaunderHeatPer);
            return CommandResult.Done(-1);
        }
    }
}