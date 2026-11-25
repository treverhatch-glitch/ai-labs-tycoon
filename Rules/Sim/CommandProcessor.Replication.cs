using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult ReplicationGate(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab puts its code out.");
            if (!Replication.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody tries to reproduce papers in this game.");
            if (lab.ReplPending < 1) return Fail(CommandError.InvalidArgument, "No paper is waiting for a decision about its code.");
            return CommandResult.Done(-1);
        }

        static CommandResult ReleaseCode(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = ReplicationGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (Replication.Running(lab)) return Fail(CommandError.InvalidArgument, "Another lab is still trying to reproduce the last paper.");
            if (cmd.Count < 0 || cmd.Count > c.Replication.MaxEffort) return Fail(CommandError.InvalidArgument, "Clean-up is from 0 to " + c.Replication.MaxEffort + " weeks.");
            long cost = Replication.EffortCost(c, cmd.Count);
            if (lab.ResearchPoints < cost) return Fail(CommandError.MissingPrerequisite, "The clean-up needs " + cost + " research points.");
            Replication.Release(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult WithholdCode(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = ReplicationGate(c, state, lab);
            if (!gate.Ok) return gate;
            Replication.Withhold(c, state, lab, 0);
            return CommandResult.Done(-1);
        }
    }
}
