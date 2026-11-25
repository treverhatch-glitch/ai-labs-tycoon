using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult Lobby(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab lobbies.");
            BlocStanding standing = lab.Bloc(cmd.Id);
            if (standing == null || c.World.Bloc(cmd.Id) == null) return Fail(CommandError.UnknownId, "There is no government \"" + cmd.Id + "\".");
            string goal = cmd.Ids.Count > 0 ? cmd.Ids[0] : "";
            if (!Lobbying.IsGoal(goal)) return Fail(CommandError.UnknownId, "Lobby for \"quiet\" or \"subsidy\", not \"" + goal + "\".");
            string name = c.World.Bloc(cmd.Id).Name;
            int left = Lobbying.CooldownLeft(state, standing);
            if (left > 0) return Fail(CommandError.LobbyCooling, "The lab lobbied " + name + " recently. It will hear from you again in " + left + (left == 1 ? " week." : " weeks."));
            if (goal == Lobbying.Quiet && standing.Attention <= 0)
                return Fail(CommandError.InvalidArgument, name + " is not paying the lab any attention, so there is nothing to quieten.");
            long cost = Lobbying.Cost(c, state, lab, goal);
            if (lab.Cash < cost) return NeedCash(lab, cost);

            Finance.Spend(lab, LedgerLine.Hiring, cost);
            string result = Lobbying.Run(c, state, lab, standing, goal);
            Events.Add(state, EventKind.Lobbied, lab.Id, -1, cost, cmd.Id + "|" + goal + "|" + result);
            return CommandResult.Done(-1);
        }
    }
}
