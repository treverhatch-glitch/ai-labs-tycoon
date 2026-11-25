using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult PullOutOfSector(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab pulls a model out of a sector.");
            if (!Pullout.Enabled(c)) return Fail(CommandError.InvalidArgument, "A model cannot be pulled out of a sector in this game.");
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Status != ModelStatus.Released) return Fail(CommandError.WrongModelStatus, model.Name + " is " + model.Status + "; only a model on sale can be pulled out.");
            if (cmd.Ids == null || cmd.Ids.Count != 1) return Fail(CommandError.InvalidArgument, "Name the one sector to leave.");
            string sectorId = cmd.Ids[0];
            if (state.Sector(sectorId) == null) return Fail(CommandError.UnknownId, "There is no sector \"" + sectorId + "\".");
            SectorEntry entry = Pullout.EntryOf(state, lab, model, sectorId);
            if (entry == null) return Fail(CommandError.InvalidArgument, model.Name + " is not on sale in " + sectorId + ".");
            long fee = Pullout.BreakFee(c, entry);
            if (lab.Cash < fee) return NeedCash(lab, fee);
            Pullout.Run(c, state, lab, model, sectorId);
            return CommandResult.Done(-1);
        }
    }
}
