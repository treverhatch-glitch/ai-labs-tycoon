using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult OpenCase(GameConfig c, GameState state, LabState lab, Command cmd, out LawCase lawCase)
        {
            lawCase = null;
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab is sued.");
            if (!Litigation.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody sues in this game.");
            lawCase = Litigation.Find(lab, cmd.TargetId);
            if (lawCase == null) return Fail(CommandError.UnknownId, "There is no claim " + cmd.TargetId + ".");
            return CommandResult.Done(-1);
        }

        static CommandResult SettleCase(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            LawCase lawCase;
            CommandResult bad = OpenCase(c, state, lab, cmd, out lawCase);
            if (!bad.Ok) return bad;
            if (lawCase.Stage != 0) return Fail(CommandError.InvalidArgument, "The hearing is over; the claim can only be appealed.");
            long cost = Litigation.SettleCost(c, lawCase);
            if (lab.Cash < cost) return NeedCash(lab, cost);
            Litigation.Settle(c, state, lab, lawCase);
            return CommandResult.Done(-1);
        }

        static CommandResult RetainFirm(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            LawCase lawCase;
            CommandResult bad = OpenCase(c, state, lab, cmd, out lawCase);
            if (!bad.Ok) return bad;
            if (lawCase.Stage != 0) return Fail(CommandError.InvalidArgument, "The hearing is over.");
            if (lawCase.Counsel == 1) return Fail(CommandError.InvalidArgument, "A firm is already retained.");
            long cost = Litigation.FirmFee(c, lawCase);
            if (lab.Cash < cost) return NeedCash(lab, cost);
            Litigation.Retain(c, lab, lawCase);
            return CommandResult.Done(-1);
        }

        static CommandResult AppealCase(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            LawCase lawCase;
            CommandResult bad = OpenCase(c, state, lab, cmd, out lawCase);
            if (!bad.Ok) return bad;
            if (lawCase.Stage != 1) return Fail(CommandError.InvalidArgument, "There is no judgment to appeal yet.");
            if (lawCase.Appealed == 1) return Fail(CommandError.InvalidArgument, "The claim was already appealed.");
            long cost = Litigation.AppealFee(c, lawCase);
            if (lab.Cash < cost) return NeedCash(lab, cost);
            Litigation.Appeal(c, state, lab, lawCase);
            return CommandResult.Done(-1);
        }

        static CommandResult BuildSecurity(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab builds security.");
            if (!Security.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody breaks in in this game.");
            if (lab.SecurityBuilding > 0) return Fail(CommandError.InvalidArgument, "Tier " + lab.SecurityBuilding + " is still being built.");
            if (lab.SecurityTier >= c.Security.MaxTier) return Fail(CommandError.InvalidArgument, "Security is already at its top tier.");
            long cost = Security.TierPrice(c, lab.SecurityTier + 1);
            if (lab.Cash < cost) return NeedCash(lab, cost);
            Security.Build(c, state, lab);
            return CommandResult.Done(-1);
        }

        static CommandResult AnswerBreach(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab is breached.");
            if (!Security.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody breaks in in this game.");
            if (!Security.Open(lab)) return Fail(CommandError.InvalidArgument, "There is no open breach.");
            if (cmd.Count < Security.Ransom || cmd.Count > Security.Lockdown) return Fail(CommandError.InvalidArgument, "Answer 0 to pay, 1 to disclose or 2 to lock down.");
            long cost = cmd.Count == Security.Ransom ? Security.RansomCost(c, lab) : cmd.Count == Security.Lockdown ? Security.LockdownCost(c, lab) : 0;
            if (cmd.Count == Security.Lockdown && lab.SecurityTier >= c.Security.MaxTier) return Fail(CommandError.InvalidArgument, "Security is already at its top tier.");
            if (cmd.Count == Security.Lockdown && lab.SecurityBuilding > 0) return Fail(CommandError.InvalidArgument, "A tier is being built; wait for it or lock down after.");
            if (lab.Cash < cost) return NeedCash(lab, cost);
            Security.Answer(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }
    }
}
