using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetCompliance(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab runs a compliance office.");
            if (cmd.Count < 0 || cmd.Count > c.Compliance.MaxLevel) return Fail(CommandError.InvalidArgument, "The compliance office is from level 0 to " + c.Compliance.MaxLevel + ".");
            if (cmd.Count == lab.ComplianceLevel) return Fail(CommandError.InvalidArgument, "The office already runs at that level.");
            if (cmd.Count > Compliance.StaffCovers(c, lab))
                return Fail(CommandError.NotEnoughStaff, "Level " + cmd.Count + " needs " + (cmd.Count * c.Compliance.StaffPerLevel) + " policy staff.");
            lab.ComplianceLevel = cmd.Count;
            return CommandResult.Done(-1);
        }
    }
}
