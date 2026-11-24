using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult RecogniseUnion(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab has a staff union.");
            if (!Labour.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no labour organising in this game.");
            if (lab.LabourStage != Labour.Organising) return Fail(CommandError.InvalidArgument, "The staff are not asking to organise.");
            Labour.Recognise(c, state, lab, true);
            return CommandResult.Done(-1);
        }

        static CommandResult ResistUnion(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab has a staff union.");
            if (!Labour.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no labour organising in this game.");
            if (lab.LabourStage != Labour.Organising) return Fail(CommandError.InvalidArgument, "The staff are not asking to organise.");
            if (lab.Cash < c.Labour.ResistCost) return NeedCash(lab, c.Labour.ResistCost);
            Labour.Resist(c, state, lab, true);
            return CommandResult.Done(-1);
        }
    }
}
