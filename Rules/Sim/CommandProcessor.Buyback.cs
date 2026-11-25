using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult BuyBackRound(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab buys its investors out.");
            if (!Buyback.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody sells their stake back in this game.");
            FundingRoundConfig round = c.Funding.Round(cmd.Id);
            if (round == null) return Fail(CommandError.UnknownId, "There is no round called " + cmd.Id + ".");
            if (!lab.FundingRounds.Contains(round.Id)) return Fail(CommandError.MissingPrerequisite, "The lab never raised " + round.Name + ".");
            if (Buyback.Bought(lab, round.Id)) return Fail(CommandError.InvalidArgument, "The " + round.Name + " investors are already gone.");
            long price = Buyback.Price(c, state, round);
            if (lab.Cash < price) return NeedCash(lab, price);
            if (Buyback.Locked(c, state, lab)) return Fail(CommandError.InvalidArgument, "The board will not hear another buyback yet.");
            Buyback.Vote(c, state, lab, round);
            return CommandResult.Done(-1);
        }
    }
}
