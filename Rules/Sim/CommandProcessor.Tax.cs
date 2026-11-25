using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult TaxGate(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab keeps tax books.");
            if (!Tax.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody pays corporate tax in this game.");
            return CommandResult.Done(-1);
        }

        static CommandResult BuildTaxStructure(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = TaxGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (cmd.Count < 0 || cmd.Count > 2) return Fail(CommandError.InvalidArgument, "A structure is tier 1 (a holding company) or 2 (an offshore one), or 0 to unwind it.");
            if (cmd.Count == lab.TaxStructure && lab.TaxBuilding == 0) return Fail(CommandError.InvalidArgument, "The lab already keeps that structure.");
            if (cmd.Count != 0 && lab.TaxBuilding != 0) return Fail(CommandError.InvalidArgument, "A structure is already being built.");
            if (cmd.Count == 0 && lab.TaxStructure == 0 && lab.TaxBuilding == 0) return Fail(CommandError.InvalidArgument, "The lab keeps no structure.");
            if (Tax.Auditing(lab)) return Fail(CommandError.InvalidArgument, "Answer the tax audit first.");
            if (cmd.Count != 0)
            {
                long price = Tax.BuildPrice(c, cmd.Count);
                if (lab.Cash < price) return NeedCash(lab, price);
            }
            Tax.Build(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult ClaimTaxCredit(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = TaxGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (Tax.CreditNow(c, lab) <= 0) return Fail(CommandError.InvalidArgument, "There is no training spend to claim a credit on yet.");
            Tax.Claim(c, state, lab);
            return CommandResult.Done(-1);
        }

        static CommandResult AnswerTaxAudit(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = TaxGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!Tax.Auditing(lab)) return Fail(CommandError.InvalidArgument, "No audit is open.");
            if (cmd.Count < Tax.Cooperate || cmd.Count > Tax.Contest) return Fail(CommandError.InvalidArgument, "Answer with 0 (cooperate), 1 (settle) or 2 (contest).");
            if (cmd.Count == Tax.Contest && lab.Cases.Count >= c.Litigation.MaxOpen) return Fail(CommandError.InvalidArgument, "The docket is full: the claim could not be filed.");
            if (cmd.Count == Tax.Contest && !Litigation.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no court to take the audit to.");
            long cost = cmd.Count == Tax.Cooperate ? Tax.CooperateCost(c, lab) : cmd.Count == Tax.Settle ? Tax.SettleCost(c, lab) : 0;
            if (lab.Cash < cost) return NeedCash(lab, cost);
            Tax.Answer(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }
    }
}
