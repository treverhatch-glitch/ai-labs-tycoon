using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetSla(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab makes a service-level pledge.");
            if (!Sla.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no service-level pledge in this game.");
            if (cmd.Count < 0 || cmd.Count > c.Sla.MaxLevel) return Fail(CommandError.InvalidArgument, "The pledge is from level 0 to " + c.Sla.MaxLevel + ".");
            int level = Sla.Level(c, lab);
            if (cmd.Count == level) return Fail(CommandError.InvalidArgument, "The pledge is already at that level.");
            if (Sla.Locked(state, lab)) return Fail(CommandError.SlaLocked, "The pledge was changed recently and stays as it is until week " + lab.SlaLockedUntil + ".");
            if (cmd.Count > level && lab.Reputation < c.Sla.MinReputation)
                return Fail(CommandError.InvalidArgument, "Customers will not take a pledge from a lab with a reputation under " + c.Sla.MinReputation + ".");
            Sla.Set(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }
    }
}
