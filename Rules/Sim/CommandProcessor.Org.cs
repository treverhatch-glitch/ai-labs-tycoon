using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetOrg(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab chooses a structure.");
            if (!Org.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody organises labs in this game.");
            if (cmd.Count < 1 || cmd.Count > 3) return Fail(CommandError.InvalidArgument, "The structure is 1 flat, 2 by function or 3 squads.");
            if (cmd.Count == lab.OrgForm) return Fail(CommandError.InvalidArgument, "The lab is already organised that way.");
            if (lab.OrgForm != 0 && state.Tick < lab.OrgSince + c.Org.CooldownWeeks) return Fail(CommandError.InvalidArgument, "The lab cannot reorganise again until week " + (lab.OrgSince + c.Org.CooldownWeeks) + ".");
            Org.Set(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }
    }
}
