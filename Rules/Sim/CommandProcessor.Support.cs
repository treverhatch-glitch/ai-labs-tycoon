using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult StaffSupportDesk(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab keeps a support desk.");
            if (!SupportDesk.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no support desk in this game.");
            if (cmd.Count < 0) return Fail(CommandError.InvalidArgument, "The desk cannot have a negative number of people.");
            int most = SupportDesk.MostPeople(c, lab);
            if (cmd.Count > most) return Fail(CommandError.NotEnoughStaff, "The desk can have " + most + " engineers: the data allows " + c.Support.MaxPeople + " and " + c.Support.KeepFree + " must stay off it.");
            if (cmd.Count == lab.SupportPeople) return Fail(CommandError.InvalidArgument, "The desk already has " + cmd.Count + " people.");
            SupportDesk.Set(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }
    }
}
