using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult AssignAccountTeam(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab assigns account teams.");
            if (!AccountTeams.Enabled(c)) return Fail(CommandError.InvalidArgument, "There are no account teams in this game.");
            SectorConfig sector = c.Sector(cmd.Id);
            if (sector == null) return Fail(CommandError.UnknownId, "There is no sector \"" + cmd.Id + "\".");
            if (cmd.Count < 0 || cmd.Count > c.Accounts.MaxPeople)
                return Fail(CommandError.InvalidArgument, "A team takes from 0 (to disband it) to " + c.Accounts.MaxPeople + " people.");
            AccountTeam team = AccountTeams.Team(lab, sector.Id);
            int now = team == null ? 0 : team.People;
            if (cmd.Count == now) return Fail(CommandError.InvalidArgument, now == 0 ? "There is no team to disband." : "The team already has " + now + " people.");
            if (cmd.Count > 0)
            {
                SectorState sectorState = state.Sector(sector.Id);
                if (sectorState == null || sectorState.Entry(lab.Id) == null) return Fail(CommandError.InvalidArgument, "The lab has no model on sale in " + sector.Name + ".");
                int allowed = AccountTeams.Allowed(c, lab);
                if (AccountTeams.Assigned(lab) - now + cmd.Count > allowed)
                    return Fail(CommandError.NotEnoughStaff, "The lab can put " + allowed + " salespeople on teams and " + (AccountTeams.Assigned(lab) - now) + " are already assigned elsewhere.");
            }
            AccountTeams.Set(c, state, lab, sector.Id, cmd.Count);
            return CommandResult.Done(-1);
        }
    }
}
