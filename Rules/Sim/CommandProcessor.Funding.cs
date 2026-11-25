using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
  public static partial class CommandProcessor
  {
    static CommandResult RaiseFunding(GameConfig c, GameState state, LabState lab, Command cmd)
    {
      if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab can raise funding.");
      if (Charter.RefusesFunding(c, lab)) return Fail(CommandError.InvalidArgument, "A nonprofit cannot raise funding.");
      FundingRoundConfig round = c.Funding.Round(cmd.Id);
      if (round == null) return Fail(CommandError.UnknownId, "There is no funding round \"" + cmd.Id + "\".");
      if (FundingSystem.HasRound(lab, round.Id)) return Fail(CommandError.InvalidArgument, round.Name + " was already raised.");
      if (!FundingSystem.PriorRoundsDone(c, lab, round))
        return Fail(CommandError.MissingPrerequisite, "Earlier funding rounds must close first.");
      int year = GameDate.Year(c.Calendar, state.Tick);
      if (year < round.OpensYear)
        return Fail(CommandError.EraLocked, round.Name + " opens in " + round.OpensYear + ".");
      if (lab.Reputation < round.MinReputation)
        return Fail(CommandError.InvalidArgument, round.Name + " needs reputation " + round.MinReputation + " or better.");
      if (WorldQueries.IsFrozen(lab, lab.HomeBlocId))
        return FrozenIn(c, lab, lab.HomeBlocId);
      FundingSystem.ApplyRaise(c, state, lab, round);
      return CommandResult.Done(-1);
    }
  }
}