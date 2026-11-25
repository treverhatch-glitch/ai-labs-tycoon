using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult CreedGate(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab states values.");
            if (!Creed.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody states values in this game.");
            return CommandResult.Done(-1);
        }

        static CommandResult SetCreed(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = CreedGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (cmd.TargetId < 1 || cmd.TargetId > 5 || cmd.Count < 1 || cmd.Count > 5 || cmd.TargetId == cmd.Count) return Fail(CommandError.InvalidArgument, "Pick two different values from 1 to 5.");
            if (Creed.Has(lab) && lab.CreedLock > 0) return Fail(CommandError.InvalidArgument, "The staff only just heard the lab's values; wait " + lab.CreedLock + " more weeks.");
            if (Creed.Has(lab) && ((lab.CreedA == cmd.TargetId && lab.CreedB == cmd.Count) || (lab.CreedA == cmd.Count && lab.CreedB == cmd.TargetId))) return Fail(CommandError.InvalidArgument, "Those are already the lab's values.");
            Creed.Set(c, state, lab, cmd.TargetId, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult HoldTownHall(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = CreedGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!Creed.Has(lab)) return Fail(CommandError.InvalidArgument, "The lab has not stated any values.");
            if (lab.CreedBand == 2) return Fail(CommandError.InvalidArgument, "Nobody will believe a town hall while the values are a joke. Own up first.");
            if (lab.CreedTown > 0) return Fail(CommandError.InvalidArgument, "The staff had a town hall on this only recently.");
            if (lab.Cash < c.Creed.TownCost) return NeedCash(lab, c.Creed.TownCost);
            Creed.TownHall(c, state, lab);
            return CommandResult.Done(-1);
        }

        static CommandResult OwnUp(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = CreedGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!Creed.Has(lab)) return Fail(CommandError.InvalidArgument, "The lab has not stated any values.");
            if (lab.CreedIntegrity >= c.Creed.OwnUpRestore) return Fail(CommandError.InvalidArgument, "The lab has nothing to own up to.");
            if (lab.CreedTown > 0) return Fail(CommandError.InvalidArgument, "The staff had this conversation only recently.");
            Creed.Own(c, state, lab);
            return CommandResult.Done(-1);
        }
    }
}
